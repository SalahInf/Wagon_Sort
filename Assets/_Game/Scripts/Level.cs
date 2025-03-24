using MatrixAlgebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GridData", menuName = "ScriptableObjects/GridData", order = 1)]
public class Level : ScriptableObject
{
    public Int2dArray matrix;
}
