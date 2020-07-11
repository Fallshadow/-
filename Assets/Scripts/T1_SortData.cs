using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class T1_SortData : MonoBehaviour
{
    public T1DataType t1DataType;
        public int index = 0;
    public List<T1_Data> t1_Datas = new List<T1_Data>();
    // Start is called before the first frame update
    void Start()
    {
        AddData(T1DataType.TDT_T3, 2);
        AddData(T1DataType.TDT_F1, 2);
        AddData(T1DataType.TDT_S2, 2);
        AddData(T1DataType.TDT_F1, 1);
        AddData(T1DataType.TDT_T3, 1);
        AddData(T1DataType.TDT_S2, 1);
        AddData(T1DataType.TDT_T3, 5);
    }
    public void AddData(T1DataType type, int indexx)
    {
        T1_Data data = new T1_Data();
        data.t1DataType = type;
        data.index = indexx;
        t1_Datas.Add(data);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("q");

            t1_Datas = t1_Datas.OrderBy(c => c.t1DataType == T1DataType.TDT_F1)
                .ThenBy(c => c.index)
                .ThenBy(c => c.t1DataType == T1DataType.TDT_S2)
                .ThenBy(c => c.t1DataType == T1DataType.TDT_T3).ToList();
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("w");

            t1_Datas = (t1_Datas.OrderBy(c => c.t1DataType == T1DataType.TDT_F1)
                .ThenBy(c => c.index))
                .ThenBy(c => c.t1DataType == T1DataType.TDT_S2)
                .ThenBy(c => c.t1DataType == T1DataType.TDT_T3).ToList();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("e");
            t1_Datas = t1_Datas.OrderBy(c => c.t1DataType == T1DataType.TDT_F1)
                .ThenByDescending
                (c => c.t1DataType == T1DataType.TDT_S2)
                .ThenBy(c => c.t1DataType == T1DataType.TDT_T3)
                                .ThenBy(c => c.index).ToList();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("r");
            t1_Datas = t1_Datas.OrderBy(c => c.index).ToList();
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            T1_Data data = new T1_Data();
            data.t1DataType = t1DataType;
            data.index = index;
            index++;
            t1_Datas.Add(data);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            foreach(var item in t1_Datas)
            {
                Debug.Log(item.index +" : "+ item.t1DataType);
            }
        }
    }
}
