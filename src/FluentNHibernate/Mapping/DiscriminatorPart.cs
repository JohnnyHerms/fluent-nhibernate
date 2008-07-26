﻿using System;
using System.Collections.Generic;
using System.Xml;

namespace ShadeTree.DomainModel.Mapping
{
    public class DiscriminatorPart<T, PARENT> : IMappingPart
    {
        private readonly string _columnName;
        private readonly List<IMappingPart> _properties;


        public DiscriminatorPart(string columnName, List<IMappingPart> _properties)
        {
            _columnName = columnName;
            this._properties = _properties;
        }

        #region IMappingPart Members

        public void Write(XmlElement classElement, IMappingVisitor visitor)
        {
            string typeString = TypeMapping.GetTypeString(typeof (T));
            classElement.AddElement("discriminator")
                .WithAtt("column", _columnName)
                .WithAtt("type", typeString);
        }

        public int Level
        {
            get { return 1; }
        }

        #endregion

        public SubClassExpression<T, SUBCLASS> SubClass<SUBCLASS>()
        {
            return new SubClassExpression<T, SUBCLASS>(this);
        }

        #region Nested type: SubClassExpression

        public class SubClassExpression<DISC, SUBCLASS>
        {
            private readonly DiscriminatorPart<T, PARENT> _parent;
            private string _discriminatorValue;

            public SubClassExpression(DiscriminatorPart<T, PARENT> parent)
            {
                _parent = parent;
            }

            public SubClassExpression<DISC, SUBCLASS> IsIdentifiedBy(DISC discriminator)
            {
                _discriminatorValue = discriminator.ToString();
                return this;
            }

            public DiscriminatorPart<T, PARENT> MapSubClassColumns(Action<SubClassPart<SUBCLASS>> action)
            {
                var subclass = new SubClassPart<SUBCLASS>(_discriminatorValue);
                action(subclass);

                _parent._properties.Add(subclass);

                return _parent;
            }
        }

        #endregion
    }
}