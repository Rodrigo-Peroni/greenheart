using UnityEngine;

// NÂO TO USANDO ESSA CLASSE!
// Agora to pros Diamantes.
// Talvez seja uam boa prática de fato tratar todos os triggers aqui.
// Mas em relação ao player ser acertado, eu to tratando isso no script do Pig. pq tipo, o porco pode ter o dano especifico dele, sei lá
public class PlayerTrigger : MonoBehaviour
{
    private Player player;

    public AudioClip sfxCoin;

    // Start is called before the first frame update
    void Start()
    {
        // O parâmetro "Player" do Find é o nome do objeto na hierarquia da Unity.
        // O parâmetro Player do GetComponent é o nome do Script (classe).
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // FEITO NO SCRIPT DO PORCO (SimplePig)
        //if (other.CompareTag("Enemy"))
        //{
        //    player.DamagePlayer();
        //}

        if (other.CompareTag("Diamonds"))
        {
            player.AddDiamonds(1);
            Hud.instance.RefreshDiamonds(player.DiamondsCount);
            SoundManager.instance.PlaySound(sfxCoin);
            Destroy(other.gameObject);
        }

        else if (other.CompareTag("PurpleDiamonds"))
        {
            player.AddDiamonds(10);
            Hud.instance.RefreshDiamonds(player.DiamondsCount);
            SoundManager.instance.PlaySound(sfxCoin);
            Destroy(other.gameObject);
        }

        else if (other.CompareTag("Heart"))
        {
            player.PickUpHeart();
            Hud.instance.RefreshLife(player.health);            
            SoundManager.instance.PlaySound(sfxCoin);
            Destroy(other.gameObject);
        }

    }
}
