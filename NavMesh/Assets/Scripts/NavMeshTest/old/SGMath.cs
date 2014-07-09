using UnityEngine;
using System.Collections;
using System;
using NavMesh;

public class Circle
{
    //Բ��
    public Vector2 center;

    //�뾶
    public float radius;

    public Circle(Vector2 cen, float r)
    {
        center = cen;
        radius = r;
    }
}

public class SGMath
{
    const float epsilon = 1e-005f;

    /// <summary>
    /// ��鸡�������
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool IsEqualZero(double data)
    {
        if (Math.Abs(data) <= epsilon)
            return true;
        else
            return false;
    }

    /// <summary>
    /// ���ض�����o�㣬��ʼ��Ϊos����ֹ��Ϊoe�ļн�, ����soe (��λ������) 
    /// ʸ��os ��ʸ�� oe��˳ʱ�뷽��,������ֵ;���򷵻ظ�ֵ 
    /// </summary>
    /// <param name="s"></param>
    /// <param name="o"></param>
    /// <param name="e"></param>
    /// <returns>���ػ���</returns>
    public static double LineRadian(Vector2 s, Vector2 o, Vector2 e)
    {
        float dx12 = s.x - o.x;
        float dy12 = s.y - o.y;
        float dx32 = e.x - o.x;
        float dy32 = e.y - o.y;

        //�Ƕȼ��㹫ʽs�� * e�� = |s|*|e|*cos��

        double cosfi = dx12 * dx32 + dy12 * dy32;

        float norm = (dx12 * dx12 + dy12 * dy12) * (dx32 * dx32 + dy32 * dy32);

        cosfi /= Math.Sqrt(norm);

        if (cosfi >= 1.0) return 0;
        if (cosfi <= -1.0) return -Math.PI;

        double angleRadian = Math.Acos(cosfi);

        // ˵��ʸ��os ��ʸ�� oe��˳ʱ�뷽�� 
        if (dx12 * dy32 - dy12 * dx32 > 0)
            return angleRadian;

        return -angleRadian;

    }

    /// <summary>
    /// ���ض�����o�㣬��ʼ��Ϊos����ֹ��Ϊoe�ļн�, ����soe 
    /// ��λ���Ƕ�!
    /// </summary>
    /// <param name="s"></param>
    /// <param name="o"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public static uint LineAngle(Vector2 s, Vector2 o, Vector2 e)
    {
        double radian = SGMath.LineRadian(s, o, e);
        uint angle = (uint)( radian / Math.PI * 180);
        return angle;
    }

    public static bool IsEqualZero(Vector2 data)
    {
        if (IsEqualZero(data.x) && IsEqualZero(data.y))
            return true;
        else
            return false;
    }

    public static bool IsEqual(Vector2 v1, Vector2 v2)
    {
        return IsEqualZero(v1 - v2);
    }

    /// <summary>
    /// ���������ε����Բ
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <returns></returns>
    public static Circle CreateCircle(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        if (!SGMath.IsEqualZero(p1.y - p2.y) ||
            !SGMath.IsEqualZero(p2.y - p3.y))
        {
            double yc, xc;
            float m1 = -(p2.x - p1.x) / (p2.y - p1.y);
            float m2 = -(p3.x - p2.x) / (p3.y - p2.y);
            double mx1 = (p1.x + p2.x) / 2.0;
            double mx2 = (p2.x + p3.x) / 2.0;
            double my1 = (p1.y + p2.y) / 2.0;
            double my2 = (p2.y + p3.y) / 2.0;

            if (SGMath.IsEqualZero(p1.y - p2.y))
            {
                xc = (p2.x + p1.x) / 2.0;
                yc = m2 * (xc - mx2) + my2;
            }
            else if (SGMath.IsEqualZero(p3.y - p2.y))
            {
                xc = (p3.x + p2.x) / 2.0;
                yc = m1 * (xc - mx1) + my1;
            }
            else
            {
                xc = (m1 * mx1 - m2 * mx2 + my2 - my1) / (m1 - m2);
                yc = m1 * (xc - mx1) + my1;
            }

            double dx = p2.x - xc;
            double dy = p2.y - yc;
            double rsqr = dx * dx + dy * dy;
            double r = Math.Sqrt(rsqr);

            return new Circle(new Vector2((float)xc, (float)yc), (float)r);
        }
        return new Circle(new Vector2(0, 0), 0);
    }

    /// <summary>
    /// ������Բ�İ�Χ��
    /// </summary>
    /// <param name="circle"></param>
    /// <returns></returns>
    public static Rect GetCircleBoundBox(Circle circle)
    {
        Rect bBox = new Rect();
        bBox.xMin = circle.center.x - circle.radius;
        bBox.xMax = circle.center.x + circle.radius;
        bBox.yMin = circle.center.y - circle.radius;
        bBox.yMax = circle.center.y + circle.radius;
        return bBox;
    }

    /// <summary>
    /// r=multiply(sp,ep,op),�õ�(sp-op)*(ep-op)�Ĳ�� 
    ///	r>0:ep��ʸ��opsp����ʱ�뷽�� 
    ///	r=0��opspep���㹲�ߣ� 
    ///	r<0:ep��ʸ��opsp��˳ʱ�뷽�� 
    /// </summary>
    /// <param name="p1">opsp</param>
    /// <param name="p2">opep</param>
    /// <returns></returns>
    public static float CrossProduct(Vector2 p1, Vector2 p2)
    {
        return (p1.x * p2.y - p1.y * p2.x);
    }

    /// <summary>
    /// r=multiply(sp,op,ep),�õ�(sp-op)*(ep-op)�Ĳ�� 
    ///	r>0:ep��ʸ��opsp����ʱ�뷽�� 
    ///	r=0��opspep���㹲�ߣ� 
    ///	r<0:ep��ʸ��opsp��˳ʱ�뷽�� 
    /// </summary>
    /// <param name="s"></param>
    /// <param name="o"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public static float CrossProduct(Vector2 s, Vector2 o,Vector2 e)
    {
        return CrossProduct(s-o,e-o);
    }

    /// <summary>
    /// �жϾ����Ƿ��ཻ
    /// </summary>
    /// <param name="rec1"></param>
    /// <param name="rec2"></param>
    /// <returns></returns>
    public static bool CheckCross(Rect rec1, Rect rec2)
    {
        Rect ret = new Rect();
        ret.xMin = Mathf.Max(rec1.xMin, rec2.xMin);
        ret.xMax = Mathf.Min(rec1.xMax, rec2.xMax);
        ret.yMin = Mathf.Max(rec1.yMin, rec2.yMin);
        ret.yMax = Mathf.Min(rec1.yMax, rec2.yMax);

        if (ret.xMin > ret.xMax || ret.yMin > ret.yMax)
        {
            // no intersection, return empty
            return false;
        }
        Debug.Log("true rec cross");
        return true;
    }

    /// <summary>
    /// ����߶��Ƿ��ཻ
    /// </summary>
    /// <param name="sp1">line1 start point</param>
    /// <param name="ep1">line1 end point</param>
    /// <param name="sp2">line2 start point</param>
    /// <param name="ep2">line2 end point</param>
    /// <returns>is cross</returns>
    public static bool CheckCross(Vector2 sp1, Vector2 ep1, Vector2 sp2, Vector2 ep2)
    {
        if (Math.Max(sp1.x, ep1.x) < Math.Min(sp2.x, ep2.x))
        {
            return false;
        }
        if (Math.Min(sp1.x, ep1.x) > Math.Max(sp2.x, ep2.x))
        {
            return false;
        }
        if (Math.Max(sp1.y, ep1.y) < Math.Min(sp2.y, ep2.y))
        {
            return false;
        }
        if (Math.Min(sp1.y, ep1.y) > Math.Max(sp2.y, ep2.y))
        {
            return false;
        }

        float temp1 = CrossProduct((sp1 - sp2), (ep2 - sp2)) * CrossProduct((ep2 - sp2), (ep1 - sp2));
        float temp2 = CrossProduct((sp2 - sp1), (ep1 - sp1)) * CrossProduct((ep1 - sp1), (ep2 - sp1));

        if ((temp1 >= 0) && (temp2 >= 0))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// ����߶κ;����ཻ
    /// </summary>
    /// <param name="linePath"></param>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static bool CheckCross(Line2D linePath, Rect rect2)
    {
        Rect lineRect = new Rect();

        if (linePath.PointStart.x < linePath.PointEnd.x)
            lineRect.xMin = linePath.PointStart.x;
        else
            lineRect.xMin = linePath.PointEnd.x;

        if (linePath.PointStart.y < linePath.PointEnd.y)
            lineRect.yMin = linePath.PointStart.y;
        else
            lineRect.yMin = linePath.PointEnd.y;

        lineRect.width = Math.Abs(linePath.PointEnd.x - linePath.PointStart.x);
        lineRect.height = Math.Abs(linePath.PointEnd.y - linePath.PointStart.y);

        return CheckCross(lineRect, rect2);
    }


    /// <summary>
    /// �����߶����ɾ���
    /// </summary>
    /// <param name="linePath"></param>
    /// <returns></returns>
    public static Rect GetRect(Line2D linePath)
    {
        Rect lineRect = new Rect();

        if (linePath.PointStart.x < linePath.PointEnd.x)
            lineRect.xMin = linePath.PointStart.x;
        else
            lineRect.xMin = linePath.PointEnd.x;

        if (linePath.PointStart.y < linePath.PointEnd.y)
            lineRect.yMin = linePath.PointStart.y;
        else
            lineRect.yMin = linePath.PointEnd.y;

        lineRect.width = Math.Abs(linePath.PointEnd.x - linePath.PointStart.x);
        lineRect.height = Math.Abs(linePath.PointEnd.y - linePath.PointStart.y);

        return lineRect;
    }

    /// <summary>
    /// ���߶��յ��ӳ�
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="tarPos"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static Vector2 GetExtendPos(Vector2 startPos, Vector2 tarPos, int length)
    {
        Vector2 newPos = tarPos;
        float slopeRate = Math.Abs((tarPos.y - startPos.y) / (tarPos.x - startPos.x));
        float xLength, yLength;
        if(slopeRate < 1)
        {
            yLength = length;
            xLength = length / slopeRate;
        }
        else
        {
            xLength = length;
            yLength = length * slopeRate;
        }

        if(tarPos.x > startPos.x)
            newPos.x += xLength;
        else
            newPos.x -= xLength;

        if (tarPos.y > startPos.y)
            newPos.y += yLength;
        else
            newPos.y -= yLength;
        
        return newPos;
    }
}