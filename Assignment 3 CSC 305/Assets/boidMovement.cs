using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boidMovement : MonoBehaviour
{
    float clock;
    Vector3 curr;
    Vector3 prev;
    int counter;
    Vector3[] spline_table;
    Vector3[] spline_points;

    // Start is called before the first frame update
    void Start()
    {
        clock = 0f;
        counter = 0;
        spline_points = new Vector3[4] {new Vector3(4f, 2f, 0f), new Vector3(0f, 3f, 4f), new Vector3(-4f, 2f, 0), new Vector3(0f, 3f, -4f)};
        create_spline_table();
        gameObject.transform.position = new Vector3(4f, 3f, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        clock += Time.deltaTime;
        if(counter == 199) {
            counter = 0;
        }
        curr = spline_table[counter];
        counter++;

        gameObject.transform.position = Vector3.Lerp(prev, curr, clock);

        gameObject.transform.LookAt(curr);
        gameObject.transform.Rotate(0, 90, 90);

        prev = curr;
        clock = 0;
    }

    //formulas from: https://en.wikipedia.org/wiki/Cubic_Hermite_spline
    float h00(float temp) {
        return (2 * temp * temp * temp) - (3 * temp * temp) + 1;
    }
    float h01(float temp) {
        return (-2 * temp * temp * temp) + (3 * temp * temp);
    }
    float h10(float temp) {
        return (temp * temp * temp) - (2 * temp * temp) + temp;
    }
    float h11(float temp) {
        return (temp * temp * temp) - (temp * temp);
    }

    Vector3 get_slope (int point_num) {
        int pt_plus = point_num;
        int pt_minus = point_num;
        Vector3 m;
        if(point_num == 0) {
            pt_minus = spline_points.Length - 1;
        }
        else {
            pt_minus -= 1;
        }
        if(point_num == (spline_points.Length - 1)) {
            pt_plus = 0;
        }
        else {
            pt_plus += 1;
        }
        //m = (spline_points[pt_plus] - spline_points[pt_minus]) / (1f - 1f/50f); //looks better but not from slides
        m = (spline_points[pt_plus] - spline_points[pt_minus]) / 2;
        return m;
    }

    //catmull-rom
    void create_spline_table() {
        spline_table = new Vector3[200];
        for(int i = 0; i < 200; i++) {
            //geodesic reparameterization
            int t = i % 50;
            float temp = 1f * t / 50f;
            if(i >= 0 && i < 50) {
                Vector3 m0 = get_slope(0);
                Vector3 m1 = get_slope(1);
                spline_table[i] = spline_points[0] * h00(temp) + m0 * h10(temp) + spline_points[1] * h01(temp) + m1 * h11(temp); 
            }
            else if (i >= 50 && i < 100) {
                Vector3 m0 = get_slope(1);
                Vector3 m1 = get_slope(2);
                spline_table[i] = spline_points[1] * h00(temp) + m0 * h10(temp) + spline_points[2] * h01(temp) + m1 * h11(temp);
            }
            else if (i >= 100 && i < 150) {
                Vector3 m0 = get_slope(2);
                Vector3 m1 = get_slope(3);
                spline_table[i] = spline_points[2] * h00(temp) + m0 * h10(temp) + spline_points[3] * h01(temp) + m1 * h11(temp);
            }
            else {
                Vector3 m0 = get_slope(3);
                Vector3 m1 = get_slope(0);
                spline_table[i] = spline_points[3] * h00(temp) + m0 * h10(temp) + spline_points[0] * h01(temp) + m1 * h11(temp);
            }
        }
    }
}
