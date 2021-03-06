// /////////////////////////////////////////////////////////////////////
//  This is free software licensed under the NUnit license. You
//  may obtain a copy of the license as well as information regarding
//  copyright ownership at http://nunit.org.    
// /////////////////////////////////////////////////////////////////////

namespace NUnit_retry
{
    using System.Reflection;

    using NUnit.Core;

    public class RetriedTestMethod : NUnitTestMethod
    {
        private readonly int requiredPassCount;

        private readonly int tryCount;

        public RetriedTestMethod(MethodInfo method, int tryCount, int requiredPassCount)
            : base(method)
        {
            this.tryCount = tryCount;
            this.requiredPassCount = requiredPassCount;
        }

        public override TestResult Run(EventListener listener, ITestFilter filter)
        {
            var successCount = 0;
            TestResult failureResult = null;

            for (var i = 0; i < this.tryCount; i++)
            {
                var result = base.Run(listener, filter);

                if (!TestFailed(result))
                {
                    if (i == 0)
                    {
                        return result;
                    }

                    if (++successCount >= this.requiredPassCount)
                    {
                        return result;
                    }
                }
                else
                {
                    failureResult = result;
                }
            }

            return failureResult;
        }

        private static bool TestFailed(TestResult result)
        {
            return result.ResultState == ResultState.Error || result.ResultState == ResultState.Failure;
        }
    }
}
