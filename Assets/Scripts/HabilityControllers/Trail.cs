using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class Trail : MonoBehaviour
{
    bool _closed;
    float _length = 0;

    [Header("Maximum and minimum allowed length of the trail (in Unity Units).")]
    public float maxLength = 3; // change to allow longer trails
    public float minLength = 1;

    List<Vector3> _points = new List<Vector3>(30);
    List<Vector2> _screenPoints = new List<Vector2>(30);
    List<GameObject> _targets = new List<GameObject>(5);
    int _pointsCount = 0;

    Vector3 _lastPoint;
    float _startTime;
    float _endTime;

    [Header("Observations")]
    public float time;
    public float length;
    public float speed;
    public bool fast;
    public bool slow;
    public float direction;
    public float directionStdDev;
    public float angle;
    public float angleStdDev;
    public bool straight;
    public bool curvy;
    public int sharp_edges;
    public bool longTrail;
    public bool shortTrail;

    [Header("Observations Config")]
    [SerializeField]
    float _slowThreshold = 1;
    [SerializeField]
    float _fastThreshold = 10;
    [SerializeField]
    float _straightThreshold = 0.3f;
    [SerializeField]
    float _curvyThreshold = 0.3f;

    public void AddPoint(Vector3 point)
    {
        if (_closed) return;

        if (_length > maxLength) {
            Close();
            return;
        }

        if (_pointsCount > 0) {
            var distance = (point - _lastPoint).magnitude;

            if (distance < 0.001) return;

            _length += distance;
        }

        _points.Add(point);
        _screenPoints.Add(Camera.main.WorldToScreenPoint(point));
        _pointsCount++;

        transform.position = _lastPoint = point;
    }
    
    public void AddTarget(GameObject gameObject)
    {
        _targets.Add(gameObject);
    }

    public bool IsClosed() { return _closed; }

    public bool Close()
    {
        if (_closed) return true;

        _endTime = Time.time;
        _closed = true;

        if (_length < minLength) {
            // This trail should be destroyed.
            return false;
        }

        Debug.Log($"Closed trail with {_pointsCount} samples, {_length} length");

        Introspect();

        return true;
    }

    void Start() {
        _startTime = Time.time;
    }


    /* Get Statistic Info */

    struct StatisticInfo {
        public statisticField horizontal;
        public statisticField vertical;
        public statisticField direction;
        public statisticField angle;
    }
    struct statisticField {
        public float median;
        public float stdDev;
    }
    const int FieldAmount = 4;

    StatisticInfo GetStatisticInfo(Vector2[] points)
    {
        float[] median = new float[FieldAmount];
        float[] stdDev = new float[FieldAmount];

        for (int i = 0; i < FieldAmount; i++) {
            median[i] = stdDev[i] = 0;
        }

        float N = (float) _pointsCount;

        const float TWO_PI = Mathf.PI * 2;

        // Cada field se corresponde con una muestra (un float[])
        // y con una función Vector2 -> float que determina cómo formar
        // la muestra a partir de `points`.
        float[][] fields = {
            new float[_pointsCount], // horizontalField   p => p.y
            new float[_pointsCount], // verticalField     p => p.x
            new float[_pointsCount], // directionField    p => La dirección del vector que va de este hacia el siguiente. 
            new float[_pointsCount]  // angleField        p => El ángulo que se forma en este vértice.
        };

        for (var i = 0; i < _pointsCount; i++) {
            var point = points[i];

            fields[0][i] = point.y;
            fields[1][i] = point.x;

            if (i == _pointsCount - 1) {
                fields[2][i] = fields[2][i - 1];
                
            } else {
                var next = points[i + 1];
                float diff_y = next.y - point.y;
                float diff_x = next.x - point.x;
                float direction = Mathf.Atan2(diff_y, diff_x);
                
                if (i != 0) {
                    var lastDirection = fields[2][i - 1];
                    while (Mathf.Abs(direction - lastDirection) > Mathf.Abs(direction + TWO_PI - lastDirection)) {
                        direction += TWO_PI;
                    }
                    while (Mathf.Abs(direction - lastDirection) > Mathf.Abs(direction - TWO_PI - lastDirection)) {
                        direction -= TWO_PI;
                    }
                }

                fields[2][i] = direction;
            }

            // We skip calculating angle[0]
            float angle = i == 0 ? 0 : fields[2][i] - fields[2][i - 1];
            
            fields[3][i] = angle;

            for (int j = 0; j < FieldAmount; j++) {
                median[j] += fields[j][i];
            }
        }

        // We incorporate angle[0]
        median[3] += fields[3][0] = fields[3][_pointsCount/2];

        for (int i = 0; i < FieldAmount; i++) {
            median[i] /= N;
        }

        for (int i = 0; i < _pointsCount; i++) {
            for (int j = 0; j < FieldAmount; j++) {
                float diff = median[j] - fields[j][i];
                diff *= diff;
                stdDev[j] += diff;
            }
        }

        for (int i = 0; i < FieldAmount; i++) {
            stdDev[i] = Mathf.Sqrt(stdDev[i] / N);
        }
        
        StatisticInfo statisticInfo;
        
        statisticInfo.horizontal.median = median[0];
        statisticInfo.vertical.median = median[1];
        statisticInfo.direction.median = median[2];
        statisticInfo.angle.median = median[3];

        statisticInfo.horizontal.stdDev = stdDev[0];
        statisticInfo.vertical.stdDev = stdDev[1];
        statisticInfo.direction.stdDev = stdDev[2];
        statisticInfo.angle.stdDev = stdDev[3];

        return statisticInfo;
    }

    void Introspect() {
        length = _length;
        time = _endTime - _startTime;
        speed = _length / time;

        slow = speed < _slowThreshold;
        fast = speed > _fastThreshold;

        Vector2[] points = _screenPoints.ToArray();
        StatisticInfo statisticInfo = GetStatisticInfo(points);

        direction = statisticInfo.direction.median;
        directionStdDev = statisticInfo.direction.stdDev;
        
        straight = directionStdDev < _straightThreshold;
        
        angle = statisticInfo.angle.median;
        angleStdDev = statisticInfo.angle.stdDev;
        
        curvy = Mathf.Abs(angle - Mathf.PI) > _curvyThreshold;

        // sharp_edges podría hacerse midiendo los valores anómalos (alejados del 1,5 RIC).
    }

    //public int sharp_edges;
    //public float longTrail;
    //public float shortTrail;

    
}
