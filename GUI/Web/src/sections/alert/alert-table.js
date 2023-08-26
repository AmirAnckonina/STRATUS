import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import {
  Box,
  Card,
  TextField,
  CardContent,
  Typography,
  Grid,
  Cell,
} from '@mui/material';
import { Scrollbar } from 'src/components/scrollbar';
import { DataGrid, GridToolbar } from '@mui/x-data-grid';
import SyncIcon from '@mui/icons-material/Sync'
import clsx from 'clsx';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend } from 'recharts';

const CustomNumberIntervalFilter = (props) => {
  const { item, applyValue, focusElementRef = null } = props;

  const filterTimeout = React.useRef();
  const [filterValueState, setFilterValueState] = React.useState(item.value ?? '');
  const [applying, setIsApplying] = React.useState(false);

  React.useEffect(() => {
    return () => {
      clearTimeout(filterTimeout.current);
    };
  }, []);

  React.useEffect(() => {
    const itemValue = item.value ?? [undefined, undefined];
    setFilterValueState(itemValue);
  }, [item.value]);

  const updateFilterValue = (lowerBound, upperBound) => {
    clearTimeout(filterTimeout.current);
    setFilterValueState([lowerBound, upperBound]);

    setIsApplying(true);
    filterTimeout.current = setTimeout(() => {
      setIsApplying(false);
      applyValue({ ...item, value: [lowerBound, upperBound] });
    }, 500);
  };

  const handleUpperFilterChange = (event) => {
    const newUpperBound = event.target.value;
    updateFilterValue(filterValueState[0], newUpperBound);
  };
  const handleLowerFilterChange = (event) => {
    const newLowerBound = event.target.value;
    updateFilterValue(newLowerBound, filterValueState[1]);
  };

  return (
    <Box
      sx={{
        display: 'inline-flex',
        flexDirection: 'row',
        alignItems: 'end',
        height: 48,
        pl: '20px',
      }}
    >
      <TextField
        name="lower-bound-input"
        placeholder="From"
        label="From"
        variant="standard"
        value={Number(filterValueState[0])}
        onChange={handleLowerFilterChange}
        type="number"
        inputRef={focusElementRef}
        sx={{ mr: 2 }}
      />
      <TextField
        name="upper-bound-input"
        placeholder="To"
        label="To"
        variant="standard"
        value={Number(filterValueState[1])}
        onChange={handleUpperFilterChange}
        type="number"
        InputProps={applying ? { endAdornment: <SyncIcon /> } : {}}
      />
    </Box>
  );
};

export const AlertsTable = (props) => {
  const { items } = props;

  const sequentialItems = items.map((item, index) => ({
    ...item,
    id: index + 1,
    percentageUsage: item.percentageUsage.toFixed(3),
  }));
  useEffect(() => {
    console.log('Items:', items);
  }, [items]);

  const columns = [
    { field: 'machineId', headerName: 'Machine IP', flex: 1 },
    {
      field: 'type',
      headerName: 'Type',
      flex: 1,      
      type: 'singleSelect',
      valueOptions: ['CPU', 'MEMORY', 'STORAGE'],
      cellClassName: (params) => {
        if (params.value == null) {
          return '';
        }
  
        return clsx('super-app', {
          cpu: params.value === 'CPU',
          storage: params.value === 'STORAGE',
          memory: params.value === 'MEMORY',
        });
      },
    },
    {
      field: 'creationTime',
      headerName: 'Alert creation time',
      flex: 1,
      type: 'date',
      valueFormatter: (params) => {
        const date = new Date(params.value);
        return date.toLocaleString();
      },
    },
    {
      field: 'percentageUsage',
      headerName: 'Average usage percentage',
      flex: 1,
      filterOperators: [
        {
          label: 'Between',
          value: 'between',
          getApplyFilterFn: (filterItem) => {
            if (!Array.isArray(filterItem.value) || filterItem.value.length !== 2) {
              return null;
            }
            if (filterItem.value[0] == null || filterItem.value[1] == null) {
              return null;
            }
            
            return ({ value }) => {
              return (
                value !== null &&
                filterItem.value[0] <= value &&
                value <= filterItem.value[1]
              );
            };
          },
          InputComponent: CustomNumberIntervalFilter,
        },
      ],
    },
    {
      field: '__check__',
      headerName: 'CheckboxSelection',
      width: 48,
      disableClickEventBubbling: true,
      renderCell: () => null,
      sortable: false,
      filterable: false,
      disableSelectionOnClick: true,
    },
  ];

  const calculateAverageUsage = (type) => {
    const typeItems = sequentialItems.filter(item => item.type === type);
    if (typeItems.length === 0) {
      return 0;
    }
    const totalUsage = typeItems.reduce((sum, item) => sum + parseFloat(item.percentageUsage), 0);
    return (totalUsage / typeItems.length).toFixed(3);
  };


  const dataForBarChart = [
    { name: 'CPU', value: sequentialItems.filter(item => item.type === 'CPU').length, color: '#1a3e72' },
    { name: 'MEMORY', value: sequentialItems.filter(item => item.type === 'MEMORY').length, color: 'lightblue' },
    { name: 'STORAGE', value: sequentialItems.filter(item => item.type === 'STORAGE').length, color: '#1a3e72' },
  ];

  const dataForPercentageBarChart = [
    { name: 'CPU', value: calculateAverageUsage('CPU'), color: '#1a3e72' },
    { name: 'MEMORY', value: calculateAverageUsage('MEMORY'), color: 'lightblue' },
    { name: 'STORAGE', value: calculateAverageUsage('STORAGE'), color: '#1a3e72' },
  ];

 
  return (
    <Card>
      <CardContent>
        <Grid container spacing={2}>
          <Grid item xs={12}>
            <Box
              sx={{
                height: 550,
                width: '100%',
                '& .super-app.cpu': {
                  backgroundColor: 'rgba(224, 183, 60, 0.55)',
                  color: '#1a3e72',
                  fontWeight: '600',
                },
                '& .super-app.storage': {
                  backgroundColor: 'rgba(157, 255, 118, 0.49)',
                  color: '#1a3e72',
                  fontWeight: '600',
                },
                '& .super-app.memory': {
                  backgroundColor: 'lightblue',
                  color: '#1a3e72',
                  fontWeight: '600',
                },
              }}
            >
              <Scrollbar>
                <div style={{ height: 500, width: '100%' }}>
                  <DataGrid
                    rows={sequentialItems}
                    columns={columns}
                    checkboxSelection
                    slots={{ toolbar: GridToolbar }}
                  />
                </div>
              </Scrollbar>
            </Box>
          </Grid>
          <Grid item xs={12}>
            <Box>
            <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                {/* Bar Chart for Count */}
                <div style={{ flex: 1 }}>
                  <Typography variant="h6" gutterBottom>
                    Type Count
                  </Typography>
                  <BarChart width={400} height={300} data={dataForBarChart}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="name" />
                    <YAxis />
                    <Tooltip />
                    <Legend />
                    <Bar dataKey="value" fill="#1a3e72">
                      {dataForBarChart.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={entry.color} />
                      ))}
                    </Bar>
                  </BarChart>
                </div>

                {/* Bar Chart for Average Usage Percentage */}
                <div style={{ flex: 1 }}>
                  <Typography variant="h6" gutterBottom>
                    Average Usage By Type
                  </Typography>
                  <BarChart width={400} height={300} data={dataForPercentageBarChart}>
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="name" />
                    <YAxis />
                    <Tooltip />
                    <Legend />
                    <Bar dataKey="value" fill="rgba(224, 183, 60, 0.55)">
                      {dataForPercentageBarChart.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={entry.color} />
                      ))}
                    </Bar>
                  </BarChart>
                </div>
              </div>
            </Box>
          </Grid>
        </Grid>
      </CardContent>
    </Card>
  );
};



AlertsTable.propTypes = {
  count: PropTypes.number,
  items: PropTypes.array.isRequired,
  onDeselectAll: PropTypes.func,
  onDeselectOne: PropTypes.func,
  onPageChange: PropTypes.func,
  onRowsPerPageChange: PropTypes.func,
  onSelectAll: PropTypes.func,
  onSelectOne: PropTypes.func,
  page: PropTypes.number,
  rowsPerPage: PropTypes.number,
  selected: PropTypes.array,
};

export default AlertsTable;
