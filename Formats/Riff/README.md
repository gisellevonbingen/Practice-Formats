# Code Example

## 1. Read file as Stream like ZipInputStream
```CS
using (var input = new RiffInputStream(new FileStream(inputFile, FileMode.Open)))
{

	while (true)
	{
		var chunk = input.ReadNextChunk();

		if (chunk == null)
		{
			break;
		}

		using (var ms = new MemoryStream())
		{
			input.CopyTo(ms);
			Console.WriteLine($"{input.CurrentPath} : {chunk.TypeKeyToString}");
		}

	}


}
```
### Output 1
```
RIFF(WAVE)[0]LIST(INFO)[0] : IART
RIFF(WAVE)[0]LIST(INFO)[1] : INAM
RIFF(WAVE)[0]LIST(INFO)[2] : IPRD
RIFF(WAVE)[0]LIST(INFO)[3] : IGNR
RIFF(WAVE)[0]LIST(INFO)[4] : ITOC
RIFF(WAVE)[0]LIST(INFO)[5] : ITRK
RIFF(WAVE)[1] : fmt
RIFF(WAVE)[2] : data
```
### Output 2
```
RIFF(ACON)[0] : anih
RIFF(ACON)[1]LIST(fram)[0] : icon
RIFF(ACON)[1]LIST(fram)[1] : icon
RIFF(ACON)[1]LIST(fram)[2] : icon
RIFF(ACON)[1]LIST(fram)[3] : icon
RIFF(ACON)[1]LIST(fram)[4] : icon
RIFF(ACON)[1]LIST(fram)[5] : icon
RIFF(ACON)[1]LIST(fram)[6] : icon
RIFF(ACON)[1]LIST(fram)[7] : icon
RIFF(ACON)[1]LIST(fram)[8] : icon
RIFF(ACON)[1]LIST(fram)[9] : icon
RIFF(ACON)[1]LIST(fram)[10] : icon
RIFF(ACON)[1]LIST(fram)[11] : icon
RIFF(ACON)[1]LIST(fram)[12] : icon
RIFF(ACON)[1]LIST(fram)[13] : icon
RIFF(ACON)[1]LIST(fram)[14] : icon
RIFF(ACON)[1]LIST(fram)[15] : icon
RIFF(ACON)[1]LIST(fram)[16] : icon
RIFF(ACON)[1]LIST(fram)[17] : icon
```

## 2. Read file as structurized object

```CS
using (var input = new FileStream(inputFile, FileMode.Open))
{
	var riff = (RiffChunkFile) RiffChunk.ReadChunk(input);
}
```

## 3. Write structurized object to File

```CS
RiffChunkFile riff = null;

using (var output = new FileStream(outputFile, FileMode.Create))
{
	RiffChunk.WriteChunk(output, riff);
}
```
