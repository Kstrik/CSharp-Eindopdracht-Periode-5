using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MLlib
{
    public abstract class ModelLoader
    {
        public ModelLoader()
        {

        }

        public abstract Model LoadModel(string filename);

        public async Task<Model> LoadModelAsync(string filename)
        {
            return await Task.Run(() => LoadModel(filename));
        }

        public List<Model> LoadModels(List<string> filenames)
        {
            List<Model> models = new List<Model>();
            foreach (string filename in filenames)
                models.Add(LoadModel(filename));

            return models;
        }

        public async Task<List<Model>> LoadModelsAsync(List<string> filenames)
        {
            List<Model> models = new List<Model>();
            List<Task> tasks = new List<Task>();
            foreach (string filename in filenames)
                tasks.Add(Task.Run(() => models.Add(LoadModel(filename))));

            await Task.WhenAll(tasks);
            return models;
        }
    }
}
