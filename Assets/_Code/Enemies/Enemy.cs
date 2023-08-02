using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public enum Type
    {
        steam, gnome,
    }

    public Type type;

    public enum Mode
    {
        patrol, hunt,
    }
    public Mode mode;

    public Enemy(Type type)
    {
        this.type = type;
        mode = Mode.patrol;
    }
}
