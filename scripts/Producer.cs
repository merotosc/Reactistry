public class Producer
{
    private const float ProductionRate = 2;
    private float elapsedTime = 0;
    private ItemType itemType = ItemType.O;

    public Direction OutputDirection { get; set; }

    public void Update(float delta)
    {
        elapsedTime += delta;

        while (elapsedTime >= ProductionRate)
        {
            Produce();
            elapsedTime -= ProductionRate;
        }
    }

    private void Produce()
    {
        // TODO: create new Item and add it to the list
    }
}
