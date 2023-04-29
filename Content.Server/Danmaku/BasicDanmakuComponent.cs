using Content.Shared.Actions.ActionTypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Utility;

namespace Content.Server.Danmaku
{
    /// <summary>
    /// This is used for...
    /// </summary>
    [RegisterComponent]
    public sealed class BasicDanmakuComponent : Component
    {
        [DataField("danmakuPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string DanmakuPrototype = "DanmakuBasic";

        [DataField("danmakuBasicAction")]
        public WorldTargetAction DanmakuBasicAction = new()
        {
            UseDelay = TimeSpan.FromSeconds(30),
            Icon = new SpriteSpecifier.Texture(new ResourcePath("Objects/Magic/magicactions.rsi/spell_default.png")),
            DisplayName = "danmaku",
            Description = "schut",
            Priority = -1,
            Range = 10,
            Event = new DanmakuBasicActionEvent(),
        };
    }
}
