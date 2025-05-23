using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Avalonia.Platform;

using OpenHarmony.NDK.Bindings.Native;

namespace Avalonia.OpenHarmony;

public class OpenHarmonyPlatformSettings : DefaultPlatformSettings
{
    public OpenHarmonyPlatformSettings()
    {
        ColorModeChanged += mode =>
        {
            OHDebugHelper.Debug("OpenHarmonyPlatformSettings" + $"ColorModeChanged: {mode}");
            OnColorValuesChanged(new PlatformColorValues()
            {
                ThemeVariant = mode switch
                {
                    ColorMode.COLOR_MODE_DARK => PlatformThemeVariant.Dark,
                    _ => PlatformThemeVariant.Light
                }
            });
        };
    }

    public override PlatformColorValues GetColorValues()
    {
        return new PlatformColorValues()
        {
            ThemeVariant = _colorMode switch
            {
                ColorMode.COLOR_MODE_DARK => PlatformThemeVariant.Dark,
                _ => PlatformThemeVariant.Light
            }
        };
    }

    private static event Action<ColorMode>? ColorModeChanged;
    private static ColorMode _colorMode;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)], EntryPoint = "setColor")]
    public static unsafe napi_value SetColor(napi_env env, napi_callback_info info)
    {
        ulong argc = 1;
        var args = stackalloc napi_value[(int)argc];
        node_api.napi_get_cb_info(env, info, &argc, args, null, null);
        int result;
        if (node_api.napi_get_value_int32(env, args[0], &result) is napi_status.napi_ok)
        {
            if (result is >= -1 and <= 1)
            {
                // .Net9 AOT的BUG，我在此处永远留下这一行注释。告诉所有人，AOT里不要玩太抽象的东西。
                // ColorModeChanged?.Invoke(_colorMode = (ColorMode)result);
                _colorMode = (ColorMode)result;
                ColorModeChanged?.Invoke(_colorMode);
            }
            else
            {
                _colorMode = ColorMode.COLOR_MODE_NOT_SET;
                ColorModeChanged?.Invoke(_colorMode);
            }
        }

        return default;
    }
}

internal enum ColorMode
{
    /**
     * The color mode is not set.
     *
     * @syscap SystemCapability.Ability.AbilityBase
     * @since 9
     */
    /**
     * The color mode is not set.
     *
     * @syscap SystemCapability.Ability.AbilityBase
     * @crossplatform
     * @since 10
     */
    /**
     * The color mode is not set.
     *
     * @syscap SystemCapability.Ability.AbilityBase
     * @crossplatform
     * @atomicservice
     * @since 11
     */
    COLOR_MODE_NOT_SET = -1,

    /**
     * Dark mode.
     *
     * @syscap SystemCapability.Ability.AbilityBase
     * @since 9
     */
    /**
     * Dark mode.
     *
     * @syscap SystemCapability.Ability.AbilityBase
     * @crossplatform
     * @since 10
     */
    /**
     * Dark mode.
     *
     * @syscap SystemCapability.Ability.AbilityBase
     * @crossplatform
     * @atomicservice
     * @since 11
     */
    COLOR_MODE_DARK = 0,

    /**
     * Light mode.
     *
     * @syscap SystemCapability.Ability.AbilityBase
     * @since 9
     */
    /**
     * Light mode.
     *
     * @syscap SystemCapability.Ability.AbilityBase
     * @crossplatform
     * @since 10
     */
    /**
     * Light mode.
     *
     * @syscap SystemCapability.Ability.AbilityBase
     * @crossplatform
     * @atomicservice
     * @since 11
     */
    COLOR_MODE_LIGHT = 1
}
