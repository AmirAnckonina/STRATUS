import React from 'react';
import PropTypes from 'prop-types';
import { format } from 'date-fns';
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
  Typography
} from '@mui/material';
import { Scrollbar } from 'src/components/scrollbar';
import { getInitials } from 'src/utils/get-initials';
import axios from 'axios';

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
    selected = []
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

  return (
    <>
      <Card>
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
                      key={alert.id}
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
                        {AlertType}
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
  items: PropTypes.array.isRequired, // Ensure items is required
  onDeselectAll: PropTypes.func,
  onDeselectOne: PropTypes.func,
  onPageChange: PropTypes.func,
  onRowsPerPageChange: PropTypes.func,
  onSelectAll: PropTypes.func,
  onSelectOne: PropTypes.func,
  page: PropTypes.number,
  rowsPerPage: PropTypes.number,
  selected: PropTypes.array
};
