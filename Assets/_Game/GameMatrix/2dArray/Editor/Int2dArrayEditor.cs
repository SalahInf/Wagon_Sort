using MatrixAlgebra;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Int2dArray))]
public class Int2dArrayEditor : PropertyDrawer
{
	public static Vector2Int[] _tmpIndexs = new Vector2Int[50];

	protected static Vector2Int cellSize = new Vector2Int(31, 31);

	private static Vector2 intSize = new Vector2(30, 30);
	private static Vector2 labelSize = new Vector2(100, 15);
	private static Vector2 ToggleSize = new Vector2(30, 30);

	private int selectColor = 0;
	private int selectIcon = 0;

	protected Int2dArray valueArrayObject;

	protected Vector2 start;
	protected Vector2 size;

	protected int x;
	protected int y;

	private int selectedX = -1, selectedY = -1;

	protected static float dragslidder;

	private static bool lineMode;
	private bool _showIndex;
	private bool _showValue;

	Texture2D[] texture2Ds;

	//GetPropertyHeight specifies the height needed when drawing an Int2dArray
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		int y = property.FindPropertyRelative("y").intValue;
		y = Mathf.Max(4, y - 1);

		var targetObject = property.serializedObject.targetObject;
		var targetObjectClassType = targetObject.GetType();
		var field = targetObjectClassType.GetField(property.propertyPath);

		valueArrayObject = (Int2dArray)field.GetValue(targetObject);
		LoadTexture();
		return Mathf.Max(350, 90 + y * 31);
	}

	//OnGUI specifies what is drawn
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		GUIStyle style = new GUIStyle(GUIStyle.none);
		style.alignment = TextAnchor.MiddleCenter;
		style.padding = new RectOffset(0, 0, 0, 0);
		SerializedProperty serializedY = property.FindPropertyRelative("y");
		SerializedProperty serializedX = property.FindPropertyRelative("x");

		x = serializedX.intValue;
		y = serializedY.intValue;

		int t = 1;
		for (int i = 0; i <= 9; i++)
		{
			DrowTrogleColor(-10, 32 * t + 5, i);
			t++;
		}

		//t = 1;

		//for (int i = 0; i <= 10; i++)
		//{
		//	DrowTrogleTexture(25, 32 * t + 5, t * 100, IconNumber(i));
		//	t++;
		//}
		//t = 1;
		//for (int i = 10; i <= 20; i++)
		//{
		//	DrowTrogleTexture(60, 32 * t + 5, (t + 10) * 100, IconNumber(i));
		//	t++;
		//}

		void DrowTrogleColor(int xPosition, int yPosition, int indexColor)
		{
			Rect Toggle = new Rect(position.min + new Vector2(xPosition, yPosition), ToggleSize);

			if (EditorGUI.Toggle(Toggle, selectColor == indexColor))
			{
				selectColor = indexColor;
				selectIcon = -1;
			}
			else if (selectColor == indexColor)
			{
				selectColor = 0;
			}
			if (selectColor == indexColor)
			{
				Rect ToggleSelecter = new Rect(position.min + new Vector2(xPosition - 1, yPosition - 1), ToggleSize + Vector2.one * 2);
				EditorGUI.DrawRect(ToggleSelecter, Color.white);
			}
			EditorGUI.DrawRect(Toggle, ColorFromNumber(indexColor));
		}

		void DrowTrogleTexture(int xPosition, int yPosition, int indexIcon, Texture2D texture2D)
		{
			Rect Toggle = new Rect(position.min + new Vector2(xPosition, yPosition), ToggleSize);

			if (EditorGUI.Toggle(Toggle, selectIcon == indexIcon / 100))
			{
				selectIcon = indexIcon / 100;
				selectColor = -1;
			}
			else if (selectIcon == indexIcon / 100)
			{
				selectIcon = -1;
			}
			if (selectIcon == indexIcon / 100)
			{
				Rect ToggleSelecter = new Rect(position.min + new Vector2(xPosition - 1, yPosition - 1), ToggleSize + Vector2.one * 2);
				EditorGUI.DrawRect(ToggleSelecter, Color.white);
			}
			EditorGUI.DrawRect(Toggle, ColorFromNumber(0));
			GUI.DrawTexture(Toggle, texture2D);
		}

		Rect rectL = new Rect(position.min + new Vector2(0, 20), labelSize);

		Rect rectXDec = new Rect(position.min + new Vector2(110, 20), intSize);
		Rect rectX = new Rect(position.min + new Vector2(140, 15), intSize);
		Rect rectXAdd = new Rect(position.min + new Vector2(170, 20), intSize);

		Rect rectYDec = new Rect(position.min + new Vector2(210, 20), intSize);
		Rect rectY = new Rect(position.min + new Vector2(240, 15), intSize);
		Rect rectYAdd = new Rect(position.min + new Vector2(270, 20), intSize);

		Rect rectYAdd2ModeLine = new Rect(position.min + new Vector2(370, 20), intSize + new Vector2(35, 0));


		EditorGUI.LabelField(rectL, "Dimensions");

		int newY = EditorGUI.IntField(rectY, y, style);
		int newX = EditorGUI.IntField(rectX, x, style);

		if (GUI.Button(rectXDec, "-", EditorStyles.miniButtonMid))
		{
			newX--;
		}

		if (GUI.Button(rectXAdd, "+", EditorStyles.miniButtonMid))
		{
			newX++;
		}

		if (GUI.Button(rectYDec, "-", EditorStyles.miniButtonMid))
		{
			newY--;
		}

		if (GUI.Button(rectYAdd, "+", EditorStyles.miniButtonMid))
		{
			newY++;
		}

		rectYAdd.x += 40;
		rectYAdd.size += Vector2.right * 20;

		bool clear = GUI.Button(rectYAdd, "Clear", EditorStyles.miniButtonMid);

		lineMode = GUI.Toggle(rectYAdd2ModeLine, lineMode, "Mode L", EditorStyles.miniButtonMid);
		rectYAdd2ModeLine.position += new Vector2(80, -5);
		rectYAdd2ModeLine.size += new Vector2(30, 0);
		_showIndex = GUI.Toggle(rectYAdd2ModeLine, _showIndex, "show index");
		rectYAdd2ModeLine.position += new Vector2(100, 0);
		_showValue = GUI.Toggle(rectYAdd2ModeLine, _showValue, "show value");

		SerializedProperty serArray = property.FindPropertyRelative("m");

		newX = Mathf.Max(1, newX);
		newY = Mathf.Max(1, newY);

		if (x != newX || y != newY)
		{
			serializedX.intValue = newX;
			serializedY.intValue = newY;
			serArray.arraySize = newX * newY;

			for (int i = x * y; i < newX * newY; i++)
			{
				SerializedProperty intProp = serArray.GetArrayElementAtIndex(i);
				intProp.boolValue = false;
			}

			if (newX != x)
			{
				Int2dArray tmpArray = new Int2dArray(valueArrayObject);
				for (int i = 0; i < tmpArray.x && i < newX; i++)
				{
					for (int j = 0; j < tmpArray.y && j < newY; j++)
					{
						int value = tmpArray[i, j];
						int index = j * newX + i;

						SerializedProperty intProp = serArray.GetArrayElementAtIndex(index);
						intProp.intValue = value;
					}
				}
			}

			int oldX = x;
			int oldY = y;

			x = newX;
			y = newY;
		}

		if (clear)
		{
			for (int i = 0; i < newX; i++)
			{
				for (int j = 0; j < newY; j++)
				{
					int index = j * newX + i;
					SerializedProperty intProp = serArray.GetArrayElementAtIndex(index);
					intProp.intValue = 1;
				}
			}
		}

		float pad = position.xMax - (x + 2) * cellSize.x;
		if (pad < position.xMin + 100)
			pad = position.xMin + 100;

		start = new Vector2(pad, position.yMin + 40f);
		size = new Vector2(cellSize.x * x + 1, cellSize.y * y + 1);
		EditorGUI.DrawRect(new Rect(start, size), Color.black);

		start += Vector2.one;
		int n = 0;

		for (int j = 0; j < y; ++j)
		{
			for (int i = 0; i < x; ++i)
			{
				Vector2 offset = new Vector2(cellSize.x * i - dragslidder, size.y - cellSize.y * (j + 1));

				Rect rectV = new Rect(start + offset, intSize);

				if (rectV.x < position.xMin + 50)
				{
					++n;
					continue;
				}

				SerializedProperty intProp = serArray.GetArrayElementAtIndex(n);
				int value = intProp.intValue % 100;
				int value2 = intProp.intValue / 100;

				bool clicked = EditorGUI.Toggle(rectV, false);
				bool isClear = true;

				{
					if (Event.current.type == EventType.MouseDrag)
					{
						Vector2 newPos = Event.current.mousePosition;

						if (Vector2.Distance(newPos - rectV.size * 0.5f, rectV.position) < rectV.size.x * 0.5f)
						{
							clicked = true;
							isClear = false;
						}
					}
				}

				if (lineMode)
				{
					if (selectedX == i && selectedY == j)
					{
						if (clicked)
						{
							selectedX = -1;
							clicked = false;
						}
						else
						{
							Rect rectV2 = new Rect(start + offset - Vector2.one * 2, intSize + Vector2.one * 4);
							EditorGUI.DrawRect(rectV2, Color.white);
						}
					}
				}

				if (clicked && selectColor != -1)
				{
					if (lineMode)
					{
						if (selectedX == -1)
						{
							value = selectColor;
							intProp.intValue = selectColor;
						}
						else
						{
							int count = Tex2DExtension.GetPointsOnLine(selectedX, selectedY, i, j, ref _tmpIndexs, 50);
							for (int i1 = 0; i1 < count; i1++)
							{
								int n2 = _tmpIndexs[i1].y * x + _tmpIndexs[i1].x;
								SerializedProperty intProp2 = serArray.GetArrayElementAtIndex(n2);
								intProp2.intValue = selectColor;
							}
						}

						selectedX = i;
						selectedY = j;

					}
					else
					{
						if (value == selectColor && isClear)
						{
							value = 0;
							intProp.intValue = 0 + value2 * 100;
						}
						else
						{
							value = selectColor;
							intProp.intValue = selectColor + value2 * 100;
						}
					}
					valueArrayObject.update = true;
				}

				EditorGUI.DrawRect(rectV, ColorFromNumber(value));

				if (!lineMode && isClear)
				{
					if (clicked && selectIcon != -1)
					{
						if (selectIcon == value2)
						{
							value2 = 0;
						}
						else
						{
							value2 = selectIcon;
						}
						intProp.intValue = value + value2 * 100;
					}
				}

				if (value2 > 0)
					GUI.DrawTexture(rectV, IconNumber(value2 - 1));

				if (_showIndex && (n + ((x + 1) % 2) * (j % 2)) % 2 == 0)
					EditorGUI.IntField(rectV, n, style);
				if (_showValue)
					EditorGUI.IntField(rectV, value, style);
				++n;
			}
		}

		OnDrowGUIDragSLidderH(start + Vector2.up * (size.y + 2), position.xMax);


		EditorGUI.EndProperty();
	}

	protected virtual void OnDrowGUIDragSLidderH(Vector2 start, float maxX)
	{
		if (size.x > maxX - 50)
		{
			Rect rect0 = new Rect(start, new Vector2(maxX + 35, 20));
			dragslidder = EditorGUI.Slider(rect0, dragslidder, 0, cellSize.x * x - maxX + 120);
		}
		else
			dragslidder = 0;
	}

	private Color ColorFromNumber(int number)
	{
		if (number < 0)
			return Color.LerpUnclamped(Color.white, Int2dArray.celllsColor[-number], 0.15f);

		if (number < Int2dArray.celllsColor.Length)
			return Int2dArray.celllsColor[number];

		return new Color(0.9f, 0.9f, 0.9f, 1);
	}

	private Texture2D IconNumber(int number)
	{
		if (number < texture2Ds.Length)
			return texture2Ds[number];

		return null;
	}

	void LoadTexture()
	{ 
		//if (texture2Ds == null)
		//{
		//	texture2Ds = new Texture2D[10];

		//	for (int i = 0; i <= 9; i++)
		//	{
		//		Texture2D texture2D = Tex2DExtension.LoadPNG("/Editor/Icons/icon-" + i + ".png");
		//		texture2D.alphaIsTransparency = true;
		//		texture2D.Apply();

		//		texture2Ds[i] = texture2D;
		//		//
		//	}
		//}
	}

}
