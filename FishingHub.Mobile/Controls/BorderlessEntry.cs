namespace FishingHub.Mobile.Controls;

// A thin subclass of Entry whose sole purpose is to opt into the "BorderlessEntry"
// handler mapping registered in BorderlessEntryHandler.Apply() — MAUI's handler
// mapping system keys off the control's type/class name, so this lets us target
// only entries that explicitly want the borderless treatment.
public class BorderlessEntry : Entry
{
}