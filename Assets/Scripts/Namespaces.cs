using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Geometry
{
    public class Cell //cell for a grid
    {
        private Bounds bounds;
        private float xMin, xMax, zMin, zMax, width, depth;

        public float value;

        //private Renderer rend;
        private Mesh mymesh;
        private MeshRenderer myrend;
        private GameObject myobj;
        private Material mymat;
        private float scaleX = 1f;
        private float scaleZ = 1f;
        private Texture2D _texture2D;
        private int i, j;
        private Vector3 _thisPos;


        public Cell(Vector2 center, Vector2 size, bool plot, Vector2 factor, Vector2 map_size, ref Texture2D texture2D,
            int i, int j)
        {
            bounds = new Bounds(
                new Vector3(center.x,
                    GameObject.Find("Floor").transform.position.y +
                    GameObject.Find("Floor").transform.localScale.y / 2f, center.y), new Vector3(size.x, 0f, size.y));
            xMin = bounds.center.x - bounds.extents.x;
            xMax = bounds.center.x + bounds.extents.x;
            zMin = bounds.center.z - bounds.extents.z;
            zMax = bounds.center.z + bounds.extents.z;
            width = xMax - xMin;
            depth = zMax - zMin;
            value = 0f;
            this.i = i;
            this.j = j;
            _thisPos = new Vector3(bounds.center.x + (map_size.x + scaleX) * factor.x, 0.1f,
                bounds.center.z + (map_size.y + scaleZ) * factor.y);
//            if (plot)
//            {
//                myobj = GameObject.CreatePrimitive(PrimitiveType.Plane);
//                myobj.transform.position = new Vector3(bounds.center.x + (map_size.x + scaleX) *factor.x, 0.1f, bounds.center.z + (map_size.y + scaleZ) * factor.y);
//                myobj.transform.localScale = new Vector3(0.1f * scaleX, 0.1f, 0.1f * scaleZ);
//                myobj.layer = 8;
//                mymat = myobj.GetComponent<Renderer>().material;
//            }

            _texture2D = texture2D;
            //myobj.GetComponent<Renderer>().bounds.size.Set(size.x, 0f, size.y);
            //myobj.GetComponent<Renderer>().bounds.center.Set(center.x, GameObject.Find("Floor").transform.position.y + GameObject.Find("Floor").transform.localScale.y / 2f, center.y);

            //rend.material.color = Color.green;
        }

        public bool Contains(Vector3 coord) //checks if cell bounds contain coord
        {
            return bounds.Contains(coord);
        }

        public void Copy(Cell cell)
        {
            bounds = cell.bounds;
            xMin = cell.xMin;
            xMax = cell.xMax;
            zMin = cell.zMin;
            zMin = cell.zMax;
            width = cell.width;
            depth = cell.depth;
            value = cell.value;
        }

        public void SetValue(float setValue)
        {
            value = setValue;
        }

        public void UpdateColor()
        {
            _texture2D.SetPixel(i, j, new Color(1 - value, value, 0));
            //if (value != 0) Debug.Log(value);
        }

        public void UpdateColorPriority()
        {
//			mymat.color = new Color(value, 1 - value,0);
            _texture2D.SetPixel(i, j, new Color(value, 1 - value, 0));
            //if (value != 0) Debug.Log(value);
        }

        public Vector3 GetPosition()
        {
            return _thisPos;
//            return myobj.transform.position;
        }


        /*public bool ContainsProjection(Vector3 coord, out Vector3 projectedPoint)//checks whether the cell contains the projected point
        {
            Plane plane = new Plane(bounds.center, bounds.center + bounds.extents, new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
            bool ret = bounds.Contains(plane.ClosestPointOnPlane(coord));
            if (ret) { projectedPoint = plane.ClosestPointOnPlane(coord); }
            else { projectedPoint = Vector3.positiveInfinity; }
            return ret;
        }

        public bool ContainsProjection(Vector3 coord)//checks whether the cell contains the projected point
        {
            Plane plane = new Plane(bounds.center, bounds.center + bounds.extents, bounds.center - bounds.extents);
            bool ret = bounds.Contains(plane.ClosestPointOnPlane(coord));
            return ret;
        }*/
    }

    [System.Serializable]
    public class
        Line //line with orientation vector which is the normal coming out of the surface on which the line is lying
    {
        public enum Axis
        {
            posX = 1,
            negX = -1,
            posZ = 2,
            negZ = -2
        };

        [SerializeField] public readonly float min, max, lying, height;
        [SerializeField] public readonly Vector3 normal;
        [SerializeField] public readonly Axis normalName;
        public float associatedProbability;

        public Vector3 MinPosition
        {
            get
            {
                if (normalName == Axis.posX || normalName == Axis.negX)
                {
                    return new Vector3(lying, height, min);
                }
                else
                {
                    return new Vector3(min, height, lying);
                }
            }
        }

        public Vector3 MaxPosition
        {
            get
            {
                if (normalName == Axis.posX || normalName == Axis.negX)
                {
                    return new Vector3(lying, height, max);
                }
                else
                {
                    return new Vector3(max, height, lying);
                }
            }
        }

        public Line(Vector3 _min, Vector3 _max, Vector3 _normal)
        {
            if (_min.y == _max.y)
            {
                height = _min.y;
                if (_min.x == _max.x)
                {
                    lying = _min.x;
                    if (_normal.x == 1)
                    {
                        normalName = Axis.posX;
                        normal = Vector3.right;
                        if (_min.z > _max.z)
                        {
                            max = _min.z;
                            min = _max.z;
                        }
                        else
                        {
                            max = _max.z;
                            min = _min.z;
                        }
                    }

                    if (_normal.x == -1)
                    {
                        normalName = Axis.negX;
                        normal = Vector3.left;
                        if (_min.z > _max.z)
                        {
                            max = _min.z;
                            min = _max.z;
                        }
                        else
                        {
                            max = _max.z;
                            min = _min.z;
                        }
                    }
                }
                else
                {
                    if (_min.z == _max.z)
                    {
                        lying = _min.z;
                        if (_normal.z == 1)
                        {
                            normalName = Axis.posZ;
                            normal = Vector3.forward;
                            if (_min.x > _max.x)
                            {
                                max = _min.x;
                                min = _max.x;
                            }
                            else
                            {
                                max = _max.x;
                                min = _min.x;
                            }
                        }

                        if (_normal.z == -1)
                        {
                            normalName = Axis.negZ;
                            normal = Vector3.back;
                            if (_min.x > _max.x)
                            {
                                max = _min.x;
                                min = _max.x;
                            }
                            else
                            {
                                max = _max.x;
                                min = _min.x;
                            }
                        }
                    }
                    else
                    {
                        min = max = 0;
                        normal = Vector3.zero;
                        normalName = Axis.negX;
                    }
                }
            }
            else
            {
                min = max = lying = 0;
                normal = Vector3.zero;
                normalName = Axis.negX;
                height = Mathf.Infinity;
            }

            associatedProbability = -1;
        }

        public Vector3 ClosestPoint(Vector3 coordinates) //returns closest point to the line
        {
            Bounds bounds = new Bounds();
            if (normalName == Axis.negX || normalName == Axis.posX)
            {
                bounds.center = new Vector3(lying, height, (max + min) / 2f);
                bounds.size = new Vector3(0, 0, max - min);
            }
            else
            {
                bounds.center = new Vector3((max + min) / 2f, height, lying);
                bounds.size = new Vector3(max - min, 0, 0);
            }

            return bounds.ClosestPoint(coordinates);
        }
    }
}

namespace Containers
{
    [System.Serializable]
    public class SolutionClass //used for the solutions, keeps a vector3 only for now
    {
        public Vector3[] worldPosition;

        public int Length
        {
            get { return worldPosition.Length; }
        }

        public SolutionClass(int positionsNumber)
        {
            worldPosition = new Vector3[positionsNumber];
        }

        public void Copy(SolutionClass position)
        {
            worldPosition = new Vector3[position.worldPosition.Length];
            for (int i = 0; i < worldPosition.Length; i++)
            {
                worldPosition[i] = position.worldPosition[i];
            }
        }

        public void Copy(GameObject[] position)
        {
            worldPosition = new Vector3[position.Length];
            for (int i = 0; i < worldPosition.Length; i++)
            {
                worldPosition[i] = position[i].transform.position;
            }
        }

        public void CopyInPosition(Vector3 position, int index)
        {
            worldPosition[index] = position;
        }
    }
}