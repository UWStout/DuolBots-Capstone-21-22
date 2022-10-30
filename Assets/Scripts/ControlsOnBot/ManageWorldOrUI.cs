using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Icons
{
    public class ManageWorldOrUI : MonoBehaviour
    {
        private eSpriteRenderingTypes type;
        private bool isSetup = false;
        private Image SetStuff;
        public void setUp(eSpriteRenderingTypes SPT)
        {
            if (isSetup) { return; }

            type = SPT;
            switch (type)
            {
                case eSpriteRenderingTypes.Canvas:
                    gameObject.AddComponent<Image>();
                    SetStuff = gameObject.GetComponent<Image>();
                    SetStuff.type = Image.Type.Filled;
                    SetStuff.fillOrigin = (int)Image.Origin360.Top;
                    Color temp = SetStuff.color;
                    temp.a = .8f;
                    SetStuff.color = temp;
                    SetStuff.preserveAspect = true;
                    SetStuff.fillClockwise = false;
                    break;
                case eSpriteRenderingTypes.World:
                    gameObject.AddComponent<SpriteRenderer>();
                    foreach (Transform t in transform)
                    {
                        t.transform.localScale = new Vector3(1, 1, 1);
                    }
                    break;
            }
            isSetup = true;
        }

        public void setImage(Sprite s)
        {
            switch (type)
            {
                case eSpriteRenderingTypes.Canvas:
                    gameObject.GetComponent<Image>().sprite = s;
                    break;
                case eSpriteRenderingTypes.World:
                    gameObject.GetComponent<SpriteRenderer>().sprite = s;
                    break;
            }
        }

        public void flipImage(direction dir)
        {
            if(type == eSpriteRenderingTypes.Canvas || dir == direction.X)
            {
                SetStuff.fillClockwise = true;
            }
            switch (type,dir)
            {
                case (eSpriteRenderingTypes.Canvas, direction.X):
                    transform.localEulerAngles += new Vector3(180, 0, 0);
                    foreach(Transform t in transform)
                    {
                        t.transform.localEulerAngles = new Vector3(180, 0, 0);
                    }
                    break;
                case (eSpriteRenderingTypes.Canvas, direction.Y):
                    transform.localEulerAngles += new Vector3(0, 180, 0);
                    foreach (Transform t in transform)
                    {
                        t.transform.localEulerAngles = new Vector3(0, 180, 0);
                    }
                    break;
                case (eSpriteRenderingTypes.World, direction.X):
                    gameObject.GetComponent<SpriteRenderer>().flipX = !gameObject.GetComponent<SpriteRenderer>().flipX;
                    break;
                case (eSpriteRenderingTypes.World, direction.Y):
                    gameObject.GetComponent<SpriteRenderer>().flipY = !gameObject.GetComponent<SpriteRenderer>().flipY;
                    break;
            }
        }
    }

    public enum eSpriteRenderingTypes
    {
        World,
        Canvas
    }
    public enum direction
    {
        X,
        Y
    }

}
