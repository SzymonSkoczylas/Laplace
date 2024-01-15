// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <algorithm>

unsigned char CalculateNewPixelValue(unsigned char* fragment, long* masks)
{
    // Initialize pixel value
    int value = 0;

    // According to the algorithm formula, initially add components calculated based on the mask values and pixel values
    for (int j = 0; j < 3; j++)
        for (int i = 0; i < 3; i++)
            value += fragment[i + j * 3] * masks[i + j * 3];

    // In case the value goes beyond the boundaries (0-255), set it to the boundary value.
    value = std::clamp<int>(value, 0, 255);

    // Return the pixel value
    return (unsigned char)value;
}

// Main function applying the LAPL1 filter.
// Takes parameters:
//   inputArrayPointer: Pointer to the input byte array (passed bitmap)
//   outputArrayPointer: Pointer to the output array (where the filtered fragment will be saved)
//   bitmapLength: The length of the bitmap
//   bitmapWidth: The width of the bitmap
//   startIndex: The starting index for filtering the fragment
//   indicesToFilter: The number of indices to be filtered
// The function filters the specified fragment and saves it to the output array.
extern "C" __declspec(dllexport) void __stdcall ApplyFilterCpp(unsigned char* inputArrayPointer, unsigned char* outputArrayPointer, int bitmapLength,
    int bitmapWidth, int startIndex, int indicesToFilter){
    // Initialize masks with values from the Laplace LAPL1 filter
    long* mask = new long[9];
    // Mask
    //  -1  0  -1
    //   0  4   0
    //  -1  0  -1
    for (int i = 0; i < 9; i++)
        if (i % 2 == 0) mask[i] = -1;
        else mask[i] = 0;
    mask[4] = 4;

    // Iterate through each index of the fragment to be filtered (in each iteration, operate on 3 indices for R, G, B)
    unsigned char* r {};
    unsigned char* g {};
    unsigned char* b {};
    for (int i = startIndex; i < startIndex + indicesToFilter; i += 3)
    {
        // Skip indices on the edges of the bitmap - do not filter them according to the algorithm.
        if ((i < bitmapWidth) || (i % bitmapWidth == 0) || (i >= bitmapLength - bitmapWidth) || ((i + 2 + 1) % bitmapWidth == 0))
            continue;

        // Initialize arrays for values of R, G, B from the 3x3 area.
        r = new unsigned char[9];
        g = new unsigned char[9];
        b = new unsigned char[9];

        // Read values from the 3x3 area around the current pixel and save them to the r, g, b arrays.
        int pixelIndex{};
        int rgbIndex{};
        for (int y = 0; y < 3; y++)
            for (int x = 0; x < 3; x++)
            {
                pixelIndex = i + (bitmapWidth * (y - 1) + (x - 1) * 3);
                rgbIndex = x * 3 + y;
                r[rgbIndex] = inputArrayPointer[pixelIndex++];
                g[rgbIndex] = inputArrayPointer[pixelIndex++];
                b[rgbIndex] = inputArrayPointer[pixelIndex];
            }

        // Save the values of the filtered pixels (for R, G, B) to the output array.
        int outputPixelIndex = i - startIndex;
        outputArrayPointer[outputPixelIndex++] = CalculateNewPixelValue(r, mask);
        outputArrayPointer[outputPixelIndex++] = CalculateNewPixelValue(g, mask);
        outputArrayPointer[outputPixelIndex] = CalculateNewPixelValue(b, mask);

        // Delete masks to prevent memory leaks.
        delete[] r;
        delete[] g;
        delete[] b;
    }

    // Delete masks to prevent memory leaks.
    delete[] mask;
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

