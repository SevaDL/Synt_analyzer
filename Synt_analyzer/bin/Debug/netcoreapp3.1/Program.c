﻿int param = 1;
int i;
int j;
for (i = 1; i < 3; i + 1)
{
	for (j = 1; j < 100; j + 1)
	{
		param = param + 1;
	}
}
int number = 1;
while (number < 3)
{
	number = number + 1;
	while (param < 3)
	{
		param = param + 1;
	}
}
do
{
	int find = 50;
	do
	{
		number = number + 1;
		param = 3;
	} while (number != find)
		number = number - 1;
} while (number > 2)