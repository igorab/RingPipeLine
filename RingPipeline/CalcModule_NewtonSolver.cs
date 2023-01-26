using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcModule_RingPipeLine
{
    public class Vector<T>
    {
        public T[] X { get; set; }

        public int Count { get; set; }
    }

    public class Matrix<T>
    {
        public T[,] Mat { get; set; }
    }

    public class DenseMatrix<T>
    {
        public static Matrix<T> Create(int _i, int _j, T _value)
        {
            Matrix<T> M = new Matrix<T>();

            return M;
        }
    }


    class TECalcModule_NewtonSolver
    {
        public Matrix<double> dF(Vector<double> _X)
        {
            Matrix<double> df = DenseMatrix<double>.Create(_X.Count, _X.Count, 0);

            return df;
        }

        public void RunSolver()
        {

        }

    }
}
