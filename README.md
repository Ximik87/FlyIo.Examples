# FlyIo.Examples

**Challenge #1**

Echo

java -jar maelstrom.jar  test -w echo --bin "FlyIo.Examples\FlyIo.Examples\bin\Debug\net8.0\FlyIo.Examples.exe" --node-count 1 --time-limit 10


**Challenge #2**

Generate unique ids

java -jar maelstrom.jar  test -w unique-ids --bin "FlyIo.Examples\FlyIo.Examples\bin\Debug\net8.0\FlyIo.Examples.exe" --time-limit 30 --rate 1000 --node-count 3 --availability total --nemesis partition


**Challenge #3a**

Broadcast for single node

java -jar maelstrom.jar  test -w broadcast --bin "FlyIo.Examples\FlyIo.Examples\bin\Debug\net8.0\FlyIo.Examples.exe" --node-count 1 --time-limit 20 --rate 10


**Challenge #3b**

Broadcast for multi node

java -jar maelstrom.jar  test -w broadcast --bin "FlyIo.Examples\FlyIo.Examples\bin\Debug\net8.0\FlyIo.Examples.exe" --node-count 5 --time-limit 20 --rate 10


**Challenge #4** (failed)

Grow-Only Counter

java -jar maelstrom.jar  test -w g-counter --bin "FlyIo.Examples\FlyIo.Examples\bin\Debug\net8.0\FlyIo.Examples.exe" --node-count 3 --rate 100 --time-limit 20 --nemesis partition