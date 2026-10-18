using System;
using R3;
using Sim.Dispositio.Shared;
using Sim.Faciem;
using Sim.Faciem.Commands;
using Unity.Properties;

namespace Sim.Utility.Editor
{
    public class SerializableGuidEditorViewModel : Bindable<SerializableGuidEditorViewModel>
    {
        private string _guid;

        [CreateProperty]
        public bool IsReadonly { get; }

        [CreateProperty]
        public Command RegenerateId { get; set; }

        [CreateProperty]
        public string PropertyName { get; set; }

        [CreateProperty]
        public string Guid
        {
            get => _guid;
            set => SetProperty(ref _guid, value);
        }

        public Observable<Maybe<Guid>> GuidChanged { get; }

        public SerializableGuidEditorViewModel(Command regenerateId, string propertyName, string initialGuid, bool isReadonly)
        {
            RegenerateId = regenerateId;
            PropertyName = propertyName;
            Guid = initialGuid;
            IsReadonly = isReadonly;

            GuidChanged = Observe(x => x.Guid)
                .Select(str => System.Guid.TryParse(str, out var guid)
                    ? Maybe.Some(guid)
                    : Maybe.None<Guid>());
        }
    }
}
