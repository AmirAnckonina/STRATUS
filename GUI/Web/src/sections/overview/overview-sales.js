import PropTypes from 'prop-types';
import ArrowPathIcon from '@heroicons/react/24/solid/ArrowPathIcon';
import ArrowRightIcon from '@heroicons/react/24/solid/ArrowRightIcon';
import axios from 'axios';
import React, { useEffect, useState } from 'react';
import { FormControl, FormControlLabel, Radio, RadioGroup } from '@mui/material';
import {LineChart, CartesianGrid, XAxis, YAxis, Legend, Tooltip, Line, AreaChart, linearGradient, Area} from 'recharts';


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


const getCategories = (filter) => {
  const today = new Date();
  const year = today.getFullYear();
  const month = today.getMonth() + 1;
  const lastDayOfMonth = new Date(year, month, 0).getDate();

  let categories = [];

  switch (filter) {
    case 'day':
      categories = Array.from({ length: 24 }, (_, i) => i + 1);
      break;
    case 'week':
      categories = Array.from({ length: 7 }, (_, i) => i + 1);
      break;
    case 'month':
      categories = Array.from({ length: lastDayOfMonth }, (_, i) => i + 1);
      break;
    case 'year':
      categories = Array.from({ length: 12 }, (_, i) => i + 1);
      break;
    default:
      break;
  }

  return categories;
};


const useChartOptions = (xAxisCategories) => {
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
      categories: xAxisCategories,
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
      min: 0,
      max: 100
    }
    
  };
};


const now = new Date();
const daysInMonth = new Date(now.getFullYear(), now.getMonth() + 1, 0).getDate();
const percentageValues = [0, 25, 50, 75, 100];
const today = new Date();
const last30Days = new Date(today.getFullYear(), today.getMonth(), today.getDate() - 30);

const dummyData = Array.from({ length: 30 }, (_, index) => {
  const date = new Date(last30Days.getFullYear(), last30Days.getMonth(), last30Days.getDate() + index);
  const formattedDate = `${date.getDate()}.${date.getMonth() + 1}`;
  return {
    x: formattedDate,
    Percentages: Math.floor(Math.random() * 101) // Generates a random number between 0 and 100
  };
});


export const OverviewSales = (props) => {
  const { selectedMachine, sx } = props;
  const chartOptions = useChartOptions();
  const [filter, setFilter] = useState('Month'); // Default filter is 'month'
  const [xAxisCategories, setXAxisCategories] = useState(getCategories(filter));
  const [cpuUsageArray, setcpuUsageArray] = useState([]);
  const [formattedData, setformattedData] = useState([]);


  useEffect(() => {
    axios.get('https://localhost:7094/GetUserInstanceCpuUsageDataOverTime?instanceId=' + selectedMachine + '&filterTime=' + filter)
    .then(response => {
      const data = response.data.data;
      console.log('over time', data);
      setcpuUsageArray(data); 
      const newData = data.map(item => ({
        x: item.date,
        Percentages: item.usage,}));
      setformattedData(newData); 
       
    })
    .catch(error => console.error(error));
    },[]);

  const handleFilterChange = (event) => {
    const newFilter = event.target.value;
    console.log('filterL', newFilter);
    console.log('machine: ', selectedMachine);
    setFilter(newFilter);

    axios.get('https://localhost:7094/GetUserInstanceCpuUsageDataOverTime?instanceId=' + selectedMachine + '&filterTime=' + newFilter)
    .then(response => {
      const data = response.data.data;
      console.log('over time', data);
      setcpuUsageArray(data); 
      const newData = data.map(item => ({
        x: item.date,
        Percentages: item.usage,}));
      setformattedData(newData); 
       
    })
    .catch(error => console.error(error));
  };

  useEffect(() => {
    setXAxisCategories(getCategories(filter));
  }, [filter]);



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
        title="CPU Maximum Usage Over Time"
      />
      <CardContent>
      <FormControl component="fieldset">
      <RadioGroup
            row
            aria-label="filter"
            name="filter"
            value={filter}
            onChange={handleFilterChange}
          >
            <FormControlLabel value="Day" control={<Radio />} label="Day" />
            <FormControlLabel value="Week" control={<Radio />} label="Week" />
            <FormControlLabel value="Month" control={<Radio />} label="Month" />
            <FormControlLabel value="Year" control={<Radio />} label="Year" />
          </RadioGroup>
        </FormControl>
        <AreaChart width={730} height={350} data={formattedData}
          margin={{ top: 10, right: 30, left: 0, bottom: 0 }}>
          <defs>
            <linearGradient id="colorUv" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#8884d8" stopOpacity={0.8}/>
              <stop offset="95%" stopColor="#8884d8" stopOpacity={0}/>
            </linearGradient>
            <linearGradient id="colorPv" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#82ca9d" stopOpacity={0.8}/>
              <stop offset="95%" stopColor="#82ca9d" stopOpacity={0}/>
            </linearGradient>
          </defs>
          <XAxis dataKey="x" />
          <YAxis />
          <CartesianGrid strokeDasharray="3 3" />
          <Tooltip />
          <Area type="monotone" dataKey="Percentages" stroke="#8884d8" fillOpacity={1} fill="url(#colorUv)" />
        </AreaChart>
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
  selectedMachine: PropTypes.string,
  sx: PropTypes.object
};
