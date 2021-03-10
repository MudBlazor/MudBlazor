// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Utilities
{
    public record TransformMatrix2D
    {
        public Double[][] Values { get; init; }

        private TransformMatrix2D()
        {

        }

        public TransformMatrix2D(Double[][] values)
        {
            if (values.Length != 3)
            {
                throw new ArgumentException("expected a 3 x 3 matrix", nameof(values));
            }

            foreach (var item in values)
            {
                if (item.Length != 3)
                {
                    throw new ArgumentException("expected a 3 x 3 matrix", nameof(values));
                }
            }

            Values = values;
        }

        public static TransformMatrix2D TranslateX(Double translateX) => Translate(translateX, 0);
        public static TransformMatrix2D TranslateY(Double translateY) => Translate(0, translateY);

        public static TransformMatrix2D Translate(Double translateX, Double translateY) =>
          new TransformMatrix2D
          {
              Values = new[] {
                    new[] { 1.0, 0.0, translateX, },
                    new[] { 0.0, 1.0, translateY, },
                    new[] { 0.0, 0.0, 1.0 }
              },
          };

        public static TransformMatrix2D ScalingX(Double scaleX) => Scaling(scaleX, 1);
        public static TransformMatrix2D ScalingY(Double scaleY) => Scaling(1, scaleY);

        public static TransformMatrix2D Scaling(Double scaleX, Double scaleY) =>
            new TransformMatrix2D
            {
                Values = new[] {
                            new[] { scaleX, 0.0, 0, },
                            new[] { 0.0, scaleY, 0, },
                            new[] { 0.0, 0.0, 1.0 }
                },
            };

        public static TransformMatrix2D MirrorYAxis =>
           new TransformMatrix2D
           {
               Values = new[] {
                            new[] { 1.0, 0.0, 0.0, },
                            new[] { 0.0, -1.0, 0.0, },
                            new[] { 0.0, 0.0, 1.0 }
               },
           };

        public static TransformMatrix2D Identity =>
           new TransformMatrix2D
           {
               Values = new[] {
                            new[] { 1.0, 0.0, 0.0, },
                            new[] { 0.0, 1.0, 0.0, },
                            new[] { 0.0, 0.0, 1.0 }
               },
           };

        public static TransformMatrix2D operator *(TransformMatrix2D first, TransformMatrix2D second)
        {
            Double[][] result = new[] { new double[3], new double[3], new double[3] };

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        result[i][j] += first.Values[i][k] * second.Values[k][j];
                    }
                }
            }

            return new TransformMatrix2D { Values = result };
        }

        public static Point2D operator *(TransformMatrix2D m, Point2D point1)
        {
            Double[] v = new[] { point1.X, point1.Y, 1.0 };

            Double[] result = new[]
            {
                m.Values[0][0]*v[0] + m.Values[0][1]*v[1] + m.Values[0][2]*v[2],
                m.Values[1][0]*v[0] + m.Values[1][1]*v[1] + m.Values[1][2]*v[2],
                m.Values[2][0]*v[0] + m.Values[2][1]*v[1] + m.Values[2][2]*v[2],
            };

            return new Point2D(result[0] / result[2], result[1] / result[2]);
        }
    }
}
