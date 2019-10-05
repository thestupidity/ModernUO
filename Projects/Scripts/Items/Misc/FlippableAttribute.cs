using System;
using Server.Targeting;

namespace Server.Items
{
  public class FlipCommandHandlers
  {
    public static void Initialize()
    {
      CommandSystem.Register("Flip", AccessLevel.GameMaster, Flip_OnCommand);
    }

    [Usage("Flip")]
    [Description("Turns an item.")]
    public static void Flip_OnCommand(CommandEventArgs e)
    {
      e.Mobile.Target = new FlipTarget();
    }

    private class FlipTarget : Target
    {
      public FlipTarget()
        : base(-1, false, TargetFlags.None)
      {
      }

      protected override void OnTarget(Mobile from, object targeted)
      {
        if (targeted is Item item)
        {
          if (item.Movable == false && from.AccessLevel == AccessLevel.Player)
            return;

          Type type = item.GetType();

          FlippableAttribute[] AttributeArray =
            (FlippableAttribute[])type.GetCustomAttributes(typeof(FlippableAttribute), false);

          if (AttributeArray.Length == 0) return;

          FlippableAttribute fa = AttributeArray[0];

          fa.Flip(item);
        }
      }
    }
  }

  [AttributeUsage(AttributeTargets.Class)]
  public class DynamicFlipingAttribute : Attribute
  {
  }

  [AttributeUsage(AttributeTargets.Class)]
  public class FlippableAttribute : Attribute
  {
    public FlippableAttribute(params int[] itemIDs) => ItemIDs = itemIDs;

    public int[] ItemIDs{ get; }

    public virtual void Flip(Item item)
    {
      if (ItemIDs == null)
      {
        try
        {
          item.GetType().GetMethod("Flip", Type.EmptyTypes)?.Invoke(item, new object[0]);
        }
        catch
        {
          // ignored
        }
      }
      else
      {
        int index = 0;
        for (int i = 0; i < ItemIDs.Length; i++)
          if (item.ItemID == ItemIDs[i])
          {
            index = i + 1;
            break;
          }

        if (index > ItemIDs.Length - 1)
          index = 0;

        item.ItemID = ItemIDs[index];
      }
    }
  }
}
