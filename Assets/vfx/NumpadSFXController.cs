using UnityEngine;

public class NumpadSFXController : MonoBehaviour
{
    public AudioClip[] sfxClips = new AudioClip[5]; // 5 seslik array
    private AudioSource[] sesKaynaklari = new AudioSource[5]; // Her ses için ayrý AudioSource

    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            sesKaynaklari[i] = gameObject.AddComponent<AudioSource>();
            sesKaynaklari[i].clip = sfxClips[i];
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
            SesCalVeyaDurdur(0);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            SesCalVeyaDurdur(1);
        if (Input.GetKeyDown(KeyCode.Keypad3))
            SesCalVeyaDurdur(2);
        if (Input.GetKeyDown(KeyCode.Keypad4))
            SesCalVeyaDurdur(3);
        if (Input.GetKeyDown(KeyCode.Keypad5))
            SesCalVeyaDurdur(4);
    }

    void SesCalVeyaDurdur(int index)
    {
        if (index < 0 || index >= sesKaynaklari.Length)
            return;

        if (sesKaynaklari[index].isPlaying)
        {
            sesKaynaklari[index].Stop(); // Çalýyorsa durdur
        }
        else
        {
            sesKaynaklari[index].Play(); // Çalmýyorsa baþlat
        }
    }
}
