using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Arc.Lib.Utils.SingletonLoader;

[Singleton]
public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemies = new();
    private void Start()
    {
        //GameObject enemy = Resources.Load<GameObject>("Enemy");
        //var t = Instantiate(enemy);
        //var levelMgr = FindObjectOfType<LevelManager>();
        //t.GetComponent<GridMovement>().Grid = levelMgr.Grid;
    }
}
