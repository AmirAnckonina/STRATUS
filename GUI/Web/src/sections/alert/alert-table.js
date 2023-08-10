import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { format } from 'date-fns';
import { getColor, Color } from '@mui/material';

import {
  Avatar,
  Box,
  Card,
  Checkbox,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TablePagination,
  TableRow,
  Typography,
  TextField,
  IconButton,
  Menu,
  MenuItem,
} from '@mui/material';
import { Scrollbar } from 'src/components/scrollbar';
import { getInitials } from 'src/utils/get-initials';
import axios from 'axios';
import ArrowDropDownIcon from '@mui/icons-material/ArrowDropDown';
import { DesktopDatePicker, MobileDatePicker } from '@mui/lab';

const alertTypeColors = {
  CPU: '#ff6666', // Darker red
  STORAGE: '#6699cc', // Darker blue
  MEMORY: '#ffff66', // Darker yellow
};

const getTextColor = (backgroundColor) => {
  const rgb = parseInt(backgroundColor.substring(1), 16);
  const r = (rgb >> 16) & 0xff;
  const g = (rgb >> 8) & 0xff;
  const b = rgb & 0xff;
  const relativeLuminance = 0.2126 * r + 0.7152 * g + 0.0722 * b;
  return relativeLuminance > 128 ? '#000000' : '#ffffff';
};

const AlertTypeDropdown = ({ selectedFilters, handleFilterChange }) => {
  const [anchorEl, setAnchorEl] = useState(null);

  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleMenuItemClick = (event, selectedFilter) => {
    handleFilterChange(event, selectedFilter);
    handleClose();
  };

  return (
    <>
      <Typography variant="subtitle1" onClick={handleClick}>
        Filter by Alert Type
        <IconButton size="small" edge="end" color="primary">
          <ArrowDropDownIcon />
        </IconButton>
      </Typography>
      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleClose}
        MenuListProps={{ disablePadding: true }}
      >
        <MenuItem onClick={(e) => handleMenuItemClick(e, 'CPU')}>
          <Checkbox
            checked={selectedFilters.includes('CPU')}
            value="CPU"
            color="primary"
          />
          <Typography>CPU</Typography>
        </MenuItem>
        <MenuItem onClick={(e) => handleMenuItemClick(e, 'STORAGE')}>
          <Checkbox
            checked={selectedFilters.includes('STORAGE')}
            value="STORAGE"
            color="primary"
          />
          <Typography>STORAGE</Typography>
        </MenuItem>
        <MenuItem onClick={(e) => handleMenuItemClick(e, 'MEMORY')}>
          <Checkbox
            checked={selectedFilters.includes('MEMORY')}
            value="MEMORY"
            color="primary"
          />
          <Typography>MEMORY</Typography>
        </MenuItem>
      </Menu>
    </>
  );
};

export const AlertsTable = (props) => {
  const {
    count = 0,
    items, // Add the items prop here
    onDeselectAll,
    onDeselectOne,
    onPageChange = () => {},
    onRowsPerPageChange,
    onSelectAll,
    onSelectOne,
    page = 0,
    rowsPerPage = 0,
    selected = [],
  } = props;

  const selectedSome = selected.length > 0 && selected.length < items.length;
  const selectedAll = items.length > 0 && selected.length === items.length;

  const handlePageChange = (event, newPage) => {
    onPageChange(newPage);
  };

  const handleRowsPerPageChange = (event) => {
    onRowsPerPageChange(parseInt(event.target.value, 10));
  };

  const handleSelectAll = (event) => {
    onSelectAll(event.target.checked);
  };

  const handleSelectOne = (event, alertId) => {
    onSelectOne(alertId);
  };

  const [selectedFilters, setSelectedFilters] = useState([]);
  const [selectedDateRange, setSelectedDateRange] = useState([null, null]);

  const handleFilterChange = (event, selectedFilter) => {
    setSelectedFilters((prevFilters) =>
      event.target.checked
        ? [...prevFilters, selectedFilter]
        : prevFilters.filter((filter) => filter !== selectedFilter)
    );
  };

  const handleDateChange = (newValue) => {
    setSelectedDateRange(newValue);
  };

  const filteredItems = items
    .filter((item) =>
      selectedFilters.length > 0 ? selectedFilters.includes(item.type) : true
    )
    .filter((item) => {
      if (selectedDateRange[0] && selectedDateRange[1]) {
        const creationTime = new Date(item.creationTime);
        return (
          creationTime >= selectedDateRange[0] &&
          creationTime <= selectedDateRange[1]
        );
      }
      return true;
    });

  return (
    <>
      <Card>
        {/* Filter with checkboxes for alert types and date-time pickers */}
        <Box sx={{ m: 2 }}>
          <Stack direction="row" alignItems="center" spacing={2}>
            <AlertTypeDropdown
              selectedFilters={selectedFilters}
              handleFilterChange={handleFilterChange}
            />
            <DesktopDatePicker
              label="Start Date"
              value={selectedDateRange[0]}
              onChange={(newValue) =>
                handleDateChange([newValue, selectedDateRange[1]])
              }
              renderInput={(params) => (
                <TextField {...params} variant="standard" />
              )}
            />
            <DesktopDatePicker
              label="End Date"
              value={selectedDateRange[1]}
              onChange={(newValue) =>
                handleDateChange([selectedDateRange[0], newValue])
              }
              renderInput={(params) => (
                <TextField {...params} variant="standard" />
              )}
            />
          </Stack>
        </Box>
        <Scrollbar>
          <Box sx={{ minWidth: 800 }}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>
                    Machine IP
                  </TableCell>
                  <TableCell>
                    Type
                  </TableCell>
                  <TableCell>
                    Creation alert time
                  </TableCell>                
                  <TableCell>
                    Average usage percentage
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {items.map((alert) => {
                  const isSelected = selected.includes(alert.machineId);
                  const AlertType = alert.type;
                  const alertMachineIP = alert.machineId;
                  const alertCreatedAt = format(new Date(alert.creationTime), 'dd/MM/yyyy HH:mm:ss');
                  const averageUsage = alert.percentageUsage.toFixed(3);

                  return (
                    <TableRow
                      hover
                      key={alert.id.increment}
                      selected={isSelected}
                    >
                      <TableCell>
                        <Stack
                          alignItems="center"
                          direction="row"
                          spacing={2}
                        >
                          <Typography variant="subtitle2">
                            {alertMachineIP}
                          </Typography>
                        </Stack>
                      </TableCell>
                      <TableCell>
                      <Typography
    style={{
      backgroundColor: alertTypeColors[AlertType],
      padding: '8px 16px',
      borderRadius: '4px',
    }}
  >
    {AlertType}
  </Typography>
                      </TableCell>
                      <TableCell>
                        {alertCreatedAt}
                      </TableCell>                    
                      <TableCell>
                        {averageUsage} %
                      </TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
          </Box>
        </Scrollbar>
      </Card>
      <TablePagination
        component="div"
        count={count}
        onPageChange={handlePageChange}
        onRowsPerPageChange={handleRowsPerPageChange}
        page={page}
        rowsPerPage={rowsPerPage}
        rowsPerPageOptions={[5, 10, 25]}
      />
    </>
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

// PropTypes for AlertTypeDropdown
AlertTypeDropdown.propTypes = {
  selectedFilters: PropTypes.array.isRequired,
  handleFilterChange: PropTypes.func.isRequired,
};
