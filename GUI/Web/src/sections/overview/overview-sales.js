import PropTypes from 'prop-types';
import ArrowPathIcon from '@heroicons/react/24/solid/ArrowPathIcon';
import ArrowRightIcon from '@heroicons/react/24/solid/ArrowRightIcon';
import axios from 'axios';
import React, { useEffect, useState } from 'react';
import {
  Button,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  Divider,
  SvgIcon
} from '@mui/material';
import { alpha, useTheme } from '@mui/material/styles';
import { Chart } from 'src/components/chart';

const useChartOptions = () => {
  const theme = useTheme();

  return {
    chart: {
      background: 'transparent',
      stacked: false,
      toolbar: {
        show: false
      }
    },
    colors: [theme.palette.primary.main, alpha(theme.palette.primary.main, 0.25)],
    dataLabels: {
      enabled: false
    },
    fill: {
      opacity: 1,
      type: 'solid'
    },
    grid: {
      borderColor: theme.palette.divider,
      strokeDashArray: 2,
      xaxis: {
        lines: {
          show: false
        }
      },
      yaxis: {
        lines: {
          show: true
        }
      }
    },
    legend: {
      show: false
    },
    plotOptions: {
      bar: {
        columnWidth: '40px'
      }
    },
    stroke: {
      colors: ['transparent'],
      show: true,
      width: 2
    },
    theme: {
      mode: theme.palette.mode
    },
    xaxis: {
      axisBorder: {
        color: theme.palette.divider,
        show: true
      },
      axisTicks: {
        color: theme.palette.divider,
        show: true
      },
      categories: getCategories(),
      labels: {
        offsetY: 5,
        style: {
          colors: theme.palette.text.secondary
        }
      }
    },
    yaxis: {
      labels: {
        formatter: (value) => `${value}%`,
        offsetX: -10,
        style: {
          colors: theme.palette.text.secondary
        }
      },
      max: 100
    }
    
  };
};

const getCategories = () => {
  const today = new Date();
  const year = today.getFullYear();
  const month = today.getMonth() + 1;
  const lastDayOfMonth = new Date(year, month, 0).getDate();

  const categories = [];

  for (let i = 1; i <= lastDayOfMonth; i++) {
    categories.push(i);
  }

  return categories;
};
const now = new Date();
const daysInMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
const percentageValues = [0, 25, 50, 75, 100];
const seriesData = Array.from({ length: percentageValues.length }, (_, i) => {
  const value = percentageValues[i];
  return Array.from({ length: daysInMonth }, (_, j) => ({
    x: `${j + 1}`,
    y: value
  }));
});

export const OverviewSales = (props) => {
  const { chartSeries, sx } = props;
  const [data, setData] = useState([]);
  const chartOptions = useChartOptions();

  useEffect(() => {
    axios.get('https://localhost:7094/GetUserInstanceCpuUsageDataOverTime')
    .then(response => {
     setData(respone.json())
     console.log("Response test", response.json());
    })
    .catch(error => {
      console.log(error);
    }); },[])
  return (
    <Card sx={sx}>
      <CardHeader
        action={(
          <Button
            color="inherit"
            size="small"
            startIcon={(
              <SvgIcon fontSize="small">
                <ArrowPathIcon />
              </SvgIcon>
            )}
          >
            Sync
          </Button>
        )}
        title="CPU Usage Over Time"
      />
      <CardContent>
        <Chart
          height={350}
          options={chartOptions}
          series={seriesData}
          type="bar"
          width="100%"
        />
      </CardContent>
      <Divider />
      <CardActions sx={{ justifyContent: 'flex-end' }}>
        <Button
          color="inherit"
          endIcon={(
            <SvgIcon fontSize="small">
              <ArrowRightIcon />
            </SvgIcon>
          )}
          size="small"
        >
          Overview
        </Button>
      </CardActions>
    </Card>
  );
};

OverviewSales.protoTypes = {
  chartSeries: PropTypes.array.isRequired,
  sx: PropTypes.object
};
