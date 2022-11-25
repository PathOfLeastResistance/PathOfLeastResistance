

// This helper function returns 1.0 if the current pixel is on a grid line, 0.0 otherwise
void IsGridLine_float(float2 fragCoord, float fwidth, out float result)
{
    // result = min( frac(fragCoord.y) , frac(fragCoord.x))
    float count = 10;
    float dist = frac(fragCoord.x * count) / fwidth / count;
    result = step(1 , dist);
}