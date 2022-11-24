using System.Threading;

namespace SysProg_AppList._2_Tasks
{
    class PoolWorkerData
    {
        public int Num { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
