// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <cmath>
#include <algorithm>
#include <iostream>
#define EXPORTED_METHOD extern "C" __declspec(dllexport)

int myMax(int a, int b) {
    return a > b ? a : b;
}
int myMin(int a, int b) {
    return a < b ? a : b;
}

EXPORTED_METHOD
int RetVal()
{
    return 20;
}

EXPORTED_METHOD
void ImageToBlack(unsigned char* imgStart, unsigned char* imgEnd)
{
    int kernel[3][3] = { {-1, 0, -1}, {0, 4, 0}, {-1, 0, -1} };
    int kernelSize = 3;
    int kernelRadius = kernelSize / 2;

    unsigned char* p = imgStart;
    int width = imgEnd - imgStart;
    int height = width / 3;

    // Create a temporary buffer to store the transformed image
    unsigned char* tempBuffer = new unsigned char[width * height * 3];

    // Apply the Laplace transformation to each pixel in the image
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x += 3)
        {
            // Calculate the Laplace value for this pixel
            int laplaceR = 0, laplaceG = 0, laplaceB = 0;
            for (int ky = 0; ky < kernelSize; ky++)
            {
                for (int kx = 0; kx < kernelSize; kx++)
                {
                    int px = x + (kx - kernelRadius) * 3;
                    int py = y + ky - kernelRadius;
                    if (px < 0 || px >= width || py < 0 || py >= height)
                    {
                        continue;
                    }

                    int kernelValue = kernel[ky][kx];
                    laplaceR += kernelValue * imgStart[py * width + px];
                    laplaceG += kernelValue * imgStart[py * width + px + 1];
                    laplaceB += kernelValue * imgStart[py * width + px + 2];
                }
            }

            // Set the pixel value in the temporary buffer
            tempBuffer[y * width + x] = (unsigned char)myMax(0, myMin(255, laplaceR));
            tempBuffer[y * width + x + 1] = (unsigned char)myMax(0, myMin(255, laplaceG));
            tempBuffer[y * width + x + 2] = (unsigned char)myMax(0, myMin(255, laplaceB));
        }
    }

    // Copy the transformed image back to the original buffer
    memcpy(imgStart, tempBuffer, width * height * 3);

    // Free the temporary buffer
    delete[] tempBuffer;
}

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}
