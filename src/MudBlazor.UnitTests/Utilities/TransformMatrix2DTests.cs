// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    public class TransformMatrix2DTests
    {
        [Test]
        public void TransformMatrix2D_ValidConstructor()
        {
            TransformMatrix2D matrix = new TransformMatrix2D(
                new[]
                {
                    new [] { 1.0, 0.0, 0.0 },
                    new [] { 0.0, 1.0, 0.0 },
                    new [] { 0.0, 0.0, 1.0 }
                });

            matrix.Values.Should().HaveCount(3);

            for (int i = 0; i < 3; i++)
            {
                matrix.Values[i].Should().HaveCount(3);
                Double[] expected = new[] { 0.0, 0.0, 0.0 };
                expected[i] = 1;

                matrix.Values[i].Should().HaveCount(3).And.ContainInOrder(expected);
            }
        }

        [Test]
        public void TransformMatrix2D_FailedConstructorNot3x3Matrix()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                TransformMatrix2D matrix = new TransformMatrix2D(
                     new[]
                     {
                    new [] { 1.0, 0.0, 0.0 },
                    new [] { 0.0, 1.0, 0.0 },
                    new [] { 0.0, 0.0, 1.0 },
                    new [] { 0.0, 0.0, 1.0 }
                     });
            });

            Assert.Throws<ArgumentException>(() =>
            {
                TransformMatrix2D matrix = new TransformMatrix2D(
                     new[]
                     {
                    new [] { 1.0, 0.0 },
                    new [] { 0.0, 1.0 },
                    new [] { 0.0, 0.0 },
                     });
            });

            Assert.Throws<ArgumentException>(() =>
            {
                TransformMatrix2D matrix = new TransformMatrix2D(
                     new[]
                     {
                    new [] { 1.0, 0.0 },
                    new [] { 0.0, 1.0, 0.0 },
                    new [] { 0.0, 0.0, 1.0 },
                     });
            });

            Assert.Throws<ArgumentException>(() =>
            {
                TransformMatrix2D matrix = new TransformMatrix2D(
                     new[]
                     {
                    new [] { 1.0, 0.0, 0.0, 1.0 },
                    new [] { 0.0, 1.0, 0.0 },
                    new [] { 0.0, 0.0, 1.0 },
                     });
            });
        }

        [Test]
        public void TransformMatrix2D_Scale()
        {
            Double sx = 10.2;
            Double sy = 20.3;

            TransformMatrix2D matrix = TransformMatrix2D.Scaling(sx, sy);

            matrix.Values.Should().HaveCount(3);

            List<Double[]> expectedRows = new()
            {
                new[] { sx, 0.0, 0.0 },
                new[] { 0.0, sy, 0.0 },
                new[] { 0.0, 0.0, 1.0 }
            };

            for (int i = 0; i < 3; i++)
            {
                matrix.Values[i].Should().HaveCount(3).And.ContainInOrder(expectedRows[i]);
            }
        }

        [Test]
        public void TransformMatrix2D_Translate()
        {
            Double tx = 10.2;
            Double ty = 20.3;

            TransformMatrix2D matrix = TransformMatrix2D.Translate(tx, ty);

            matrix.Values.Should().HaveCount(3);

            List<Double[]> expectedRows = new()
            {
                new[] { 1.0, 0.0, tx },
                new[] { 0.0, 1.0, ty },
                new[] { 0.0, 0.0, 1.0 }
            };

            for (int i = 0; i < 3; i++)
            {
                matrix.Values[i].Should().HaveCount(3).And.ContainInOrder(expectedRows[i]);
            }
        }

        [Test]
        public void TransformMatrix2D_MirrowYAxis()
        {
            TransformMatrix2D matrix = TransformMatrix2D.MirrorYAxis;

            matrix.Values.Should().HaveCount(3);

            List<Double[]> expectedRows = new()
            {
                new[] { 1.0, 0.0, 0.0 },
                new[] { 0.0, -1.0, 0.0 },
                new[] { 0.0, 0.0, 1.0 }
            };

            for (int i = 0; i < 3; i++)
            {
                matrix.Values[i].Should().HaveCount(3).And.ContainInOrder(expectedRows[i]);
            }
        }

        [Test]
        public void TransformMatrix2D_MatrixScalarMatrxMultiplcation()
        {
            TransformMatrix2D a = new TransformMatrix2D(
                new[]
                {
                    new [] { 5.0, 2.0, 8.0 },
                    new [] { 6.0, 6.0, 0.0 },
                    new [] { 2.0, 5.0, 9.0 }
                });

                TransformMatrix2D b = new TransformMatrix2D(
                new[]
                {
                    new [] { 9.0, 4.0, 6.0 },
                    new [] { 1.0, -9.0, 8.0 },
                    new [] { 2.0, 2.0, -3.0 }
                });

            List<Double[]> expectedRows = new()
            {
                new[] { 63.0, 18.0, 22.0 },
                new[] { 60.0, -30.0, 84.0 },
                new[] { 41.0, -19, 25 }
            };

            TransformMatrix2D a_scalar_b = a * b;

            a_scalar_b.Values.Should().HaveCount(3);

            for (int i = 0; i < 3; i++)
            {
                a_scalar_b.Values[i].Should().HaveCount(3).And.ContainInOrder(expectedRows[i]);
            }
        }

        [Test]
        public void TransformMatrix2D_MatrixScalarPoint()
        {
            TransformMatrix2D a = new TransformMatrix2D(
                new[]
                {
                    new [] { 1.0, 2.0, 1.0 },
                    new [] { 6.0, 1.0, -1.0 },
                    new [] { 0.0, 2.0, 1.0 }
                });

            Point2D p = new Point2D(-4, 5);
            Point2D expectedResult = new Point2D(7.0 / 11.0, -20.0 / 11.0);

            Point2D a_scalar_p = a * p;

            a_scalar_p.Should().Be(expectedResult);
        }

        [Test]
        public void TransformMatrix2D_ChainedTransformation()
        {
            TransformMatrix2D translate = TransformMatrix2D.Translate(0, -100);
            TransformMatrix2D mirrorY = TransformMatrix2D.MirrorYAxis;

            Point2D p = new Point2D(20, 60);
            Point2D expectedResult = new Point2D(20, 40);

            Point2D p_projection = (mirrorY * translate) * p;
            Point2D p_oppositeProjection = (translate * mirrorY) * p;

            p_projection.Should().Be(expectedResult);
            p_oppositeProjection.Should().NotBe(expectedResult);
        }
    }
}
