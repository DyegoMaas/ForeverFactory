using System.Collections.Generic;

namespace ForeverFactory.Transforms
{
    internal class TransformList<T>
    {
        private readonly List<Transform<T>> _transforms = new List<Transform<T>>();

        public void AddRange(IEnumerable<Transform<T>> transforms)
        {
            _transforms.AddRange(transforms);
        }
        
        public void AddRange(TransformList<T> transformList)
        {
            _transforms.AddRange(transformList._transforms);
        }

        public void Add(Transform<T> transform)
        {
            _transforms.Add(transform);
        }

        public TransformList<T> Union(TransformList<T> otherList)
        {
            var newList = new TransformList<T>();
            newList.AddRange(_transforms);
            newList.AddRange(otherList._transforms);
            return newList;
        }

        public IEnumerable<Transform<T>> AsEnumerable()
        {
            return _transforms;
        }
    }
}