import React, { useEffect, useRef } from 'react';
import Chart from 'chart.js';

const LineChart = ({ data }) => {
  const chartRef = useRef(null);

  useEffect(() => {
    const chartCanvas = chartRef.current.getContext('2d');
    
    // Create the chart
    new Chart(chartCanvas, {
      type: 'line',
      data: {
        labels: data.labels,
        datasets: [
          {
            label: 'Data',
            data: data.values,
            backgroundColor: 'rgba(75, 192, 192, 0.2)',
            borderColor: 'rgba(75, 192, 192, 1)',
            borderWidth: 1
          }
        ]
      },
      options: {
        scales: {
          y: {
            beginAtZero: true
          }
        }
      }
    });
  }, [data]);

  return <canvas ref={chartRef} />;
};

export default LineChart;