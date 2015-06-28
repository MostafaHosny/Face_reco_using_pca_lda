using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace lda_pca_face_rec
{
    [Serializable()]
    class ObjectToSerialize :ISerializable
    {
        private LDA lda;

        public LDA Lda 
        {
            get { return this.lda; }
            set { this.lda = value; }
        }

        public ObjectToSerialize()
        {}
        public ObjectToSerialize(SerializationInfo info, StreamingContext ctxt)
        {
            this.lda = (LDA)info.GetValue("Lda", typeof(LDA));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Lda", this.lda);
        }
    }
}
