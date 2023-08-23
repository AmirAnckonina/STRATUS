import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import {
  Box,
  Card,
  TextField,
} from '@mui/material';
import { Scrollbar } from 'src/components/scrollbar';
import { DataGrid, GridToolbar } from '@mui/x-data-grid';
import SyncIcon from '@mui/icons-material/Sync'

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
    },
    {
      field: 'creationTime',
      headerName: 'Creation alert time',
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

  return (
    <Card>
      <Scrollbar>
        <div style={{ height: 400, width: '100%' }}>
          <DataGrid
            rows={sequentialItems}
            columns={columns}
            checkboxSelection
            slots={{ toolbar: GridToolbar }}
          />
        </div>
      </Scrollbar>
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
