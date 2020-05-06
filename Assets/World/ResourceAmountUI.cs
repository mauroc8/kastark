using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(AudioSource))]
public class ResourceAmountUI : UpdateAsStream
{
    private StateStream<int> amount =
        new StateStream<int>(0);

    void Awake()
    {
        var text =
            GetComponent<TextMeshProUGUI>();

        var audioSource =
            gameObject.GetComponent<AudioSource>();

        amount
            .Get(value =>
            {
                text.text = $"{value}";

                audioSource.pitch =
                    Random.Range(0.9f, 1.1f);

                audioSource.Play();
            });

        // Animate text growing

        amount
            .Map(_ => Time.time)
            .AndThen(update.Always)
            .Get(time =>
            {
                var duration = 0.3f;
                var t = (Time.time - time) / duration;

                if (t >= 1)
                    return;

                t =
                    Mathf.Sin(t * Mathf.PI);

                transform.localScale =
                    Vector3.Lerp(
                        Vector3.one,
                        new Vector3(1.5f, 1.5f, 1.5f),
                        t
                    );
            });
    }

    public void SetResourceAmount(int amount)
    {
        if (amount != this.amount.Value)
            this.amount.Push(amount);
    }
}

