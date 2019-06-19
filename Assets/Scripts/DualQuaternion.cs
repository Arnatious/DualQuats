/* Copyright (c) 2019 Ted Kern
 * 
 * This 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuaternionExtensions {
    public static class QuatMath
    {
        public static float Sinc(float x)
        {
            return Mathf.Approximately(x, 0) ? 1 : Mathf.Sin(x) / x;
        }

        public static float Norm(this Quaternion q)
        {
            return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        }

        public static Quaternion Conjugate(this Quaternion q)
        {
            return new Quaternion(-q.x, -q.y, -q.z, q.w);
        }

        public static Quaternion Exp(this Quaternion q)
        {
            float a = Norm(q);
            float exp_w = Mathf.Exp(q.w);

            if (Mathf.Approximately(a, 0.0f))
            {
                return new Quaternion(0, 0, 0, exp_w);
            }
            float sinc = Sinc(a);
            return new Quaternion(sinc * q.x, sinc * q.y, sinc * q.z, exp_w * Mathf.Cos(a));
        }

        public static Quaternion Log(this Quaternion q)
        {
            float exp_w = Norm(q);
            float w = Mathf.Log(exp_w);
            float a = Mathf.Acos(q.w / exp_w);

            if (Mathf.Approximately(a, 0))
            {
                return new Quaternion(0, 0, 0, w);
            }

            float mag = 1 / exp_w / Sinc(a);
            return new Quaternion(q.x * mag, q.y * mag, q.z * mag, w);
        }

        public static Quaternion Add(this Quaternion A, Quaternion B)
        {
            return new Quaternion(
                A.x + B.x,
                A.y + B.y,
                A.z + B.z,
                A.w + B.w
            );
        }

        public static Quaternion Sub(this Quaternion A, Quaternion B)
        {
            return new Quaternion(
                A.x - B.x,
                A.y - B.y,
                A.z - B.z,
                A.w - B.w
            );
        }

        public static Quaternion Scale(this Quaternion B, float a)
        {
            return new Quaternion(B.x * a, B.y * a, B.z * a, B.w * a);
        }

        public static float Dot(this Quaternion a, Quaternion b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }
    }
}

namespace DualQuaternions {
    using QuaternionExtensions;
    public struct DualQuaternion
    {
        /* 
         * Original Copyright (c) 2018 Max Kaufmann
         * Extensions are Copyright (c) Ted Kern, based on neka-nat's dq3d python library
         * All code licensed under MIT
         * 
         * Permission is hereby granted, free of charge, to any person obtaining a copy
         * of this software and associated documentation files (the "Software"), to deal
         * in the Software without restriction, including without limitation the rights
         * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
         * copies of the Software, and to permit persons to whom the Software is
         * furnished to do so, subject to the following conditions:
         * The above copyright notice and this permission notice shall be included in all
         * copies or substantial portions of the Software.
         * 
         */

        public Quaternion Real;
        public Quaternion Dual;

        public static DualQuaternion Zero
        {
            get
            {
                DualQuaternion q;
                q.Real = new Quaternion(0, 0, 0, 0);
                q.Dual = new Quaternion(0, 0, 0, 0);
                return q;
            }
        }

        public DualQuaternion(Quaternion aReal, Quaternion aDual)
        {
            Real = aReal;
            Dual = aDual;
        }

        public DualQuaternion(Vector3 aPosition, Quaternion aRotation)
        {
            var v = 0.5f * aPosition;
            var q1 = new DualQuaternion(new Quaternion(0f, 0f, 0f, 1f), new Quaternion(v.x, v.y, v.z, 0f));
            var q2 = new DualQuaternion(aRotation, new Quaternion(0f, 0f, 0f, 0f));
            var q = q1 * q2;
            Real = q.Real;
            Dual = q.Dual;
        }



        public static DualQuaternion operator +(DualQuaternion A, DualQuaternion B)
        {
            return new DualQuaternion(
                A.Real.Add(B.Real),
                A.Dual.Add(B.Dual)
            );
        }

        public static DualQuaternion operator *(DualQuaternion A, DualQuaternion B)
        {
            return new DualQuaternion(
                A.Real * B.Real,
                (A.Dual * B.Real).Add(B.Dual * A.Real)
            );
        }


        public static DualQuaternion operator *(float A, DualQuaternion B)
        {
            return new DualQuaternion(B.Real.Scale(A), B.Dual.Scale(A));
        }

        public DualQuaternion Normalized
        {
            get
            {
                DualQuaternion res;
                var length = Real.Norm();
                res.Real = Real.Scale(1/length);
                res.Dual = Dual.Scale(1/length);
                res.Dual = res.Dual.Sub(res.Real.Scale(res.Real.Dot(res.Dual) * (length * length)));
                return res;
            }
        }

        public Vector3 Translation
        {
            get
            {
                var Conjugate = Real.Conjugate();
                var TQ = Dual * Conjugate;
                return new Vector3(
                    TQ.x + TQ.x,
                    TQ.y + TQ.y,
                    TQ.z + TQ.z
                );
            }
        }

        public DualQuaternion Conjugate {
            get
            {
                DualQuaternion q;
                q.Real = Real.Conjugate();
                q.Dual = Dual.Conjugate();
                return q;
            }
        }

        public DualQuaternion Log
        {
            get
            {
                DualQuaternion q;
                float scale = 1 / Real.Norm();
                scale *= scale;
                q.Real = Real.Log();
                q.Dual = (Real.Conjugate() * Dual).Scale(scale);

                return q;
            }
        }

        public DualQuaternion Exp
        {
            get
            {
                DualQuaternion q;
                q.Real = Real.Exp();
                q.Dual = q.Real * Dual;

                return q;
            }
        }

        public DualQuaternion Pow(float t)
        {
            return (t * Log).Exp;
        }

        public Vector3 TransformPosition(Vector3 Position)
        {
            return Translation + Real * Position;
        }

        public Vector3 TransformVector(Vector3 Vector)
        {
            return Real * Vector;
        }

        public DualQuaternion Sclerp(DualQuaternion Other, float t)
        {
            return (this * (Conjugate * Other).Pow(t)).Normalized;
        }
    }
}