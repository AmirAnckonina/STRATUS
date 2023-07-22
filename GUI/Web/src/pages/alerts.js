import React, { useEffect, useState, useCallback, useMemo } from 'react';
import Head from 'next/head';
import { format } from 'date-fns';
import ArrowDownOnSquareIcon from '@heroicons/react/24/solid/ArrowDownOnSquareIcon';
import ArrowUpOnSquareIcon from '@heroicons/react/24/solid/ArrowUpOnSquareIcon';
import SettingsIcon from '@heroicons/react/24/solid/Cog6ToothIcon';
import { Box, Button, Container, Stack, SvgIcon, Typography, TextField, Dialog, DialogTitle, DialogContent, DialogActions } from '@mui/material';
import { useSelection } from 'src/hooks/use-selection';
import { Layout as DashboardLayout } from 'src/layouts/dashboard/layout';
import { AlertsTable } from 'src/sections/alert/alert-table';
import { AlertsSearch } from 'src/sections/alert/alert-search';
import { applyPagination } from 'src/utils/apply-pagination';
import axios from 'axios';

const now = new Date();

const useAlerts = (Alerts, page, rowsPerPage) => {
  const startIndex = page * rowsPerPage;
  const endIndex = startIndex + rowsPerPage;
  return useMemo(() => {
    return Alerts.slice(startIndex, endIndex);
  }, [Alerts, page, rowsPerPage]);
};


const Page = () => {
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(5);
  const [isConfigFormOpen, setConfigFormOpen] = useState(false);
  const [cpuThreshold, setCpuThreshold] = useState('');
  const [memoryThreshold, setMemoryThreshold] = useState('');
  const [diskThreshold, setDiskThreshold] = useState('');
  const [cpuThresholdError, setCpuThresholdError] = useState('');
  const [memoryThresholdError, setMemoryThresholdError] = useState('');
  const [diskThresholdError, setDiskThresholdError] = useState('');
  const [intervalPeriod, setIntervalPeriod] = useState('day');
  const [intervalPeriodValue, setIntervalPeriodValue] = useState('1');
  const [intervalValueError, setIntervalValueError] = useState('');
  const [prevCpuThreshold, setPrevCpuThreshold] = useState('');
  const [prevMemoryThreshold, setPrevMemoryThreshold] = useState('');
  const [prevDiskThreshold, setPrevDiskThreshold] = useState('');
  const [prevIntervalValue, setPrevIntervalValue] = useState({
    period: 'day',
    value: 1,
  });

  const [Alerts, setAlerts] = useState([]);

  const AlertsIds = useMemo(() => {
    return Alerts.map((Alert) => Alert.id);
  }, [Alerts]);

  const AlertsSelection = useSelection(AlertsIds);
  const paginatedAlerts = useAlerts(Alerts, page, rowsPerPage);

  useEffect(() => {
    // Fetch data using Axios when the component mounts
    axios
      .get('https://localhost:7094/GetAlerts')
      .then((response) => {
        console.log('data:', response.data);
        setAlerts(response.data.data);
      })
      .catch((error) => {
        console.error('Error fetching data:', error);
      });
  }, []);

  const handlePageChange = useCallback((event, value) => {
    console.log("pages ", event);
    setPage(event);
  }, []);

  const handleRowsPerPageChange = useCallback((event) => {
    console.log("pages ", event);
    setRowsPerPage(parseInt(event, 10));
  }, []);
  

  const handleConfigFormOpen = () => {
    setConfigFormOpen(true);
    // Populate the form fields with previous configuration values
    setCpuThreshold(prevCpuThreshold);
    setMemoryThreshold(prevMemoryThreshold);
    setDiskThreshold(prevDiskThreshold);
    setIntervalPeriod(prevIntervalValue.period); // Set the interval period
    setIntervalPeriodValue(prevIntervalValue.value.toString()); // Set the interval value as a string
  };

  const handleConfigFormClose = () => {
    setConfigFormOpen(false);
    // Save the current configuration values as previous values
    setPrevCpuThreshold(cpuThreshold);
    setPrevMemoryThreshold(memoryThreshold);
    setPrevDiskThreshold(diskThreshold);
    setPrevIntervalValue({ period: intervalPeriod, value: parseInt(intervalPeriodValue) }); // Save both period and value
  };

  const handleConfigFormSubmit = async () => {
    // Reset previous errors
    setCpuThresholdError('');
    setMemoryThresholdError('');
    setDiskThresholdError('');
    setIntervalValueError('');

    // Validation for CPU Threshold, Memory Threshold, Disk Threshold, and Interval Value
    if (cpuThreshold === '' || isNaN(cpuThreshold) || cpuThreshold < 0 || cpuThreshold > 100) {
      setCpuThresholdError('Please enter a valid CPU Threshold (0 to 100)');
      return;
    }

    if (memoryThreshold === '' || isNaN(memoryThreshold) || memoryThreshold < 0 || memoryThreshold > 100) {
      setMemoryThresholdError('Please enter a valid Memory Threshold (0 to 100)');
      return;
    }

    if (diskThreshold === '' || isNaN(diskThreshold) || diskThreshold < 0 || diskThreshold > 100) {
      setDiskThresholdError('Please enter a valid Disk Threshold (0 to 100)');
      return;
    }

    // Convert intervalPeriodValue to a number before validation
    const intervalValueAsNumber = parseInt(intervalPeriodValue, 10);

    // Validation for Interval Value
    if (isNaN(intervalValueAsNumber) || intervalValueAsNumber < 1) {
      setIntervalValueError('Please enter a valid Interval Value (greater than 1)');
      return;
    }

    // If there are no errors, proceed with the HTTP POST request
    try {
      const response = await axios.post('https://localhost:7094/SetConfigurations', {
        cpuThreshold,
        memoryThreshold,
        diskThreshold,
        intervalPeriod,
        intervalPeriodValue: intervalValueAsNumber, // Use the numeric value for intervalPeriodValue
      });

      // Assuming the server responds with a success message or data, you can handle it here.
      console.log('Response from server:', response.data);
      handleConfigFormClose(); // Close the form after successful submission or handle it according to your requirement
    } catch (error) {
      // Handle any errors that occur during the request
      console.error('Error sending configuration:', error);
      // You can display an error message to the user if necessary
    }
  };

  return (
    <>
      <Head>
        <title>
          Alerts | STRATUS
        </title>
      </Head>
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          py: 8
        }}
      >
        <Container maxWidth="xl">
          <Stack spacing={3}>
            <Stack
              direction="row"
              justifyContent="space-between"
              spacing={4}
            >
              <Stack spacing={1}>
                <Typography variant="h4">
                  Alerts
                </Typography>
                <Stack
                  alignItems="center"
                  direction="row"
                  spacing={1}
                >
                  <Button
                    color="inherit"
                    startIcon={(
                      <SvgIcon fontSize="small">
                        <ArrowUpOnSquareIcon />
                      </SvgIcon>
                    )}
                  >
                    Import
                  </Button>
                  <Button
                    color="inherit"
                    startIcon={(
                      <SvgIcon fontSize="small">
                        <ArrowDownOnSquareIcon />
                      </SvgIcon>
                    )}
                  >
                    Export
                  </Button>
                </Stack>
              </Stack>
              <div>
                <Button
                  startIcon={(
                    <SvgIcon fontSize="small">
                      <SettingsIcon />
                    </SvgIcon>
                  )}
                  variant="contained"
                  onClick={handleConfigFormOpen} // Open the configuration form on button click
                >
                  Configurations
                </Button>
              </div>
            </Stack>
            <AlertsSearch />
            <AlertsTable
              count={Alerts.length} // Use the total length of Alerts for count
              items={paginatedAlerts} // Use the paginatedAlerts here
              onDeselectAll={AlertsSelection.handleDeselectAll}
              onDeselectOne={AlertsSelection.handleDeselectOne}
              onPageChange={handlePageChange}
              onRowsPerPageChange={handleRowsPerPageChange}
              onSelectAll={AlertsSelection.handleSelectAll}
              onSelectOne={AlertsSelection.handleSelectOne}
              page={page}
              rowsPerPage={rowsPerPage}
              selected={AlertsSelection.selected}
            />
          </Stack>
        </Container>
      </Box>
      <Dialog open={isConfigFormOpen} onClose={handleConfigFormClose}>
        <DialogTitle>Configurations</DialogTitle>
        <DialogContent>
          {/* ... (Form fields remain the same) */}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleConfigFormClose}>Cancel</Button>
          <Button onClick={handleConfigFormSubmit} color="primary">Confirm</Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

Page.getLayout = (page) => (
  <DashboardLayout>
    {page}
  </DashboardLayout>
);

export default Page;
