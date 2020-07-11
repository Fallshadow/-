using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum T1DataType
{
    TDT_F1 = 0,
    TDT_S2 = 1,
    TDT_T3 = 2,

}
public class T1_Data
{
    public T1DataType t1DataType = T1DataType.TDT_F1;
    public int index;
}
