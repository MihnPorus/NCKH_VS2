#pragma strict

function Start () {

}

function Update () {
if (Input.GetButton("Horizontal")||Input.GetButton("Vertical"))
{GetComponent.<AudioSource>().enabled=true;}
else
{GetComponent.<AudioSource>().enabled=false;}

}