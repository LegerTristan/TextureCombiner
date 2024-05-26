namespace TextureCombiner.Source.Datas.Utils
{
    public class SingletonTemplate<TSingleton> where TSingleton : new()
    {
        static TSingleton instance = default(TSingleton);

        public static TSingleton Instance
        {
            get
            {
                if (instance == null)
                    instance = new TSingleton();

                return instance;
            }
        }
    }
}
