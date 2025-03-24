#if UNITY_EDITOR
    using UnityEngine;
#endif
namespace MatrixAlgebra
{
    [System.Serializable]
    public class Int2dArray : Array2D<int>
    {
#if UNITY_EDITOR
        public static Color[] celllsColor = new Color[]
        {
  
            new Color (0.8f, 0.8f, 0.8f), // cells
            new Color (.6f, .6f, .6f), // obstacles      
            new Color (0f, 0f, 0f), //Entrence
            new Color (0.7552816f, 0.2941176f, .9411765f), // purpel
            new Color (.8490566f, .1161446f, .1161446f),// red
            new Color (1f, 0.7607843f, 0.2431372f),// Orange
            new Color (0.2977357f, 0.6084167f, 0.9924528f), // Bleu

        };

     

        [System.NonSerialized] public bool update;
#endif

        public Int2dArray(int width, int height) : base(width, height)
        {
        }

        public Int2dArray(int width, int height, int[] elemnts) : base(width, height, elemnts)
        {
        }

        public Int2dArray(Int2dArray bool2DArray) : base(bool2DArray)
        {

        }
    }
}