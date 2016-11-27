using UnityEngine;
using System.Collections;

public class EnemyAI : Character {

    private float lastshottime = 0f;
    private float lastmovetime = 0f;
    public float shotinterval = 0.5f;
    private float actualshotinterval = 0f;
    public float moveinterval = 1.5f;
    private Vector2 cur_movedir = Vector2.zero;
    // Use this for initialization
    void Start () {
        base.Character_Start();
        actualshotinterval = shotinterval;

    }

    // Update is called once per frame
    void Update () {
        base.Character_Update();
        move(getMoveDir());

        //Debug.Log(turndir);
        turn(getTurnDir());
        if (Time.time - lastshottime >= actualshotinterval)
        {

            fire();
            actualshotinterval = Random.Range(shotinterval* 0.5f, shotinterval *2f);
            lastshottime = Time.time;
        }
    }


    public override Vector2 getMoveDir()
    {
        if (Time.time - lastmovetime >= moveinterval)
        {
            Vector2 movedir = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f));
            lastmovetime = Time.time;
            cur_movedir = movedir;
        }
        
        return cur_movedir;
    }

    public override Vector2 getTurnDir()
    {
        Vector3 mousepoint = GameManager.player.transform.position;
        //Debug.Log(mousepoint);

        Vector2 turndir = mousepoint - new Vector3(transform.position.x, transform.position.y);
        return turndir;
    }

    public override void hit(float damage)
    {
        base.hit(damage);
        GameManager.addHitEnemy();
    }

    public override bool isEnemy()
    {
        return true;
    }



}
