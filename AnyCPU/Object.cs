namespace AnyCPU
{
    public class Object
    {
        public string Platform
        {
            get
            {
                AnyCPUInterop.Object obj = new AnyCPUInterop.Object();               
                return obj.Platform;
            }
        }
    }
}
