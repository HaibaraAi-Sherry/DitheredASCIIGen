using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DitheringASCImage {
    public class FileMonitor {
        public bool IsFileChanged {
            get; private set;
        }

        /// <summary>
        /// 保存文件所触发的事件
        /// </summary>
        public event Func<bool> SaveFileEvent;
        /// <summary>
        /// 请求询问是否保存文件的事件
        /// </summary>
        public event Func<bool> RequestToAskSave;

        public FileMonitor(Func<bool> save, Func<bool> ask) {
            SaveFileEvent += save;
            IsFileChanged = false;

            RequestToAskSave += ask;
        }

        private bool FileMonitor_RequestToAskSave() {
            throw new NotImplementedException();
        }

        public void Change() {
            IsFileChanged = true;
        }

        public void Save() {
            IsFileChanged = false;
            SaveFileEvent.Invoke();
        }

        public void CreateNewFile() {
            if (!IsFileChanged) {
                return;
            }

            if (RequestToAskSave.Invoke()) {
                SaveFileEvent.Invoke();
                IsFileChanged = false;
            }
        }
    }
}
