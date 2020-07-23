#pragma once

//random
#define rm 0x100000000LL
#define rc 0xB16
#define ra 0x5DEECE66DLL

static unsigned long long seed = 1;

double drand48(void)
{
	seed = (ra * seed + rc) & 0xFFFFFFFFFFFFLL;
	unsigned int x = seed >> 16;
	return 	((double)x / (double)rm);

}