import Head from 'next/head';
import { subDays, subHours } from 'date-fns';
import { Box, Container, Unstable_Grid2 as Grid } from '@mui/material';
import { Layout as DashboardLayout } from 'src/layouts/dashboard/layout';
import { OverviewAverageDiskUsage } from 'src/sections/overview/overview-average-disk-usage';
import { OverviewLatestMachines } from 'src/sections/overview/overview-latest-machines';
import { OverviewCpuGraph } from 'src/sections/overview/overview-cpu-graph';
import { OverviewAverageCpuUsage } from 'src/sections/overview/overview-average-cpu-usage';
import { OverviewMaximumCpuUsage } from 'src/sections/overview/overview-maximum-cpu-usage';
import { OverviewMemorySizeUsage } from 'src/sections/overview/overview-avergae-memory-usage';
import { OverviewCpuUtilization } from 'src/sections/overview/overview-cpu-utilization';
import React, { useState, useEffect } from 'react';
import axios from 'axios';

import {
  Typography,
  Select,
  MenuItem
} from '@mui/material';

const now = new Date();
axios.defaults.withCredentials = true;
const Page = () => {
  const [statistics, setStatistics] = useState([]);
  const [AvgCpuPercentage, setAvgCpu] = useState();
  const [MaxCpuPercentage, setMaxCpu] = useState();
  const [AvgDiskSpacePercentage, setAvgDiskSpace] = useState();
  const [AvgMemorySizePercentage, setAvgMemorySize] = useState();
  const [machines, setMachine] = useState([]);
  const [selectedMachine, setSelectedMachine] = useState('');
  const [cpuUtilizations, setcpuUtilizations] = useState([]);

  useEffect(() => {
    console.log(document.cookie);
    console.log("hi");
    axios.get('https://localhost:7094/GetAllUserResourcesDetails')
      .then(response => {
        if (!response.data) {
          throw new Error('Data not received');
        }
        const fetchedData = response.data.data;
        setMachine(fetchedData);
      })
      .catch(error => {
        console.error('Axios error:', error);
      });
  }, []); 

  /*useEffect(() => {
    
    axios.get('https://localhost:7094/GetAvgCpuUtilizationByCpu?instance=' + selectedMachine, {
      withCredentials: true // Include cookies in the request
    })
      .then(response => {
        const data = response.data.data;
        setcpuUtilizations(data);
      })
      .catch(error => console.error(error));
  }, []);*/

  const handleMachineChange = (event) => {
    
    setSelectedMachine(event);

    axios.get('https://localhost:7094/GetAvgCpuUsageUtilization?instance=' + event, {
      withCredentials: true // Include cookies in the request
    })
      .then(response => {
        setAvgCpu(response.data.data); 
      })
      .catch(error => console.error(error));
    
    axios.get('https://localhost:7094/GetMaxCpuUsageUtilization?instance=' + event, {
      withCredentials: true // Include cookies in the request
    })
      .then(response => {
        setMaxCpu(response.data.data); 
      })
      .catch(error => console.error(error));
    
    axios.get('https://localhost:7094/GetAvgDiskSpaceUsagePercentage?instance=' + event, {
      withCredentials: true // Include cookies in the request
    })
      .then(response => {
        setAvgDiskSpace(response.data.data); 
      })
      .catch(error => console.error(error));
    
    axios.get('https://localhost:7094/GetAvgMemorySizeUsagePercentage?instance=' + event, {
      withCredentials: true // Include cookies in the request
    })
      .then(response => {
        setAvgMemorySize(response.data.data); 
      })
      .catch(error => console.error(error));

    axios.get('https://localhost:7094/GetAvgCpuUtilizationByCpu?instance=' + event, {
      withCredentials: true// Include cookies in the request
    })
      .then(response => {
        const data = response.data.data;
        setcpuUtilizations(data);
      })
      .catch(error => console.error(error));
  };

  return (
    <>
      <Head>
        <title>
          Overview | STRATUS
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
          <Grid
            container
            spacing={3}
          >
            <Grid
              xs={12}
              md={12}
              lg={16}
            >
              <OverviewLatestMachines
                onMachineSelect={handleMachineChange}
              />
            </Grid>
            <Grid container spacing={4}>
            <Grid
              xs={12}
              sm={6}
              lg={3}
            >
              <OverviewAverageCpuUsage
                sx={{ height: '100%', witdh: '100%' }}
                value={selectedMachine ? AvgCpuPercentage ? AvgCpuPercentage.toFixed(2) : "N/A" : "N/A"}
              />
            </Grid>
            <Grid
              xs={12}
              sm={6}
              lg={3}
            >
              <OverviewMaximumCpuUsage
                difference={16}
                positive={false}
                sx={{ height: '100%', witdh: '100%'}}
                value={selectedMachine ? MaxCpuPercentage ? MaxCpuPercentage.toFixed(2) : "N/A" : "N/A"}
              />
            </Grid>      
            <Grid
              xs={12}
              sm={6}
              lg={3}
            >
              <OverviewAverageDiskUsage
                difference={12}
                positive
                sx={{ height: '100%', witdh: '100%' }}
                value={selectedMachine ? AvgDiskSpacePercentage ? AvgDiskSpacePercentage.toFixed(2) : "N/A" : "N/A"}
              />
            </Grid>               
            <Grid
              xs={12}
              sm={6}
              lg={3}
            >
              <OverviewMemorySizeUsage
                sx={{ height: '100%', witdh: '100%' }}
                value={selectedMachine ? AvgMemorySizePercentage ? AvgMemorySizePercentage.toFixed(2) : "N/A" : "N/A"}
              />
            </Grid>
            </Grid>
            <Grid xs={12} lg={8}>
              {selectedMachine ? (
                <OverviewCpuGraph selectedMachine={selectedMachine} />
              ) : (
                <div>Loading...</div>
              )}
            </Grid>
            <Grid
              xs={12}
              md={6}
              lg={4}
            >
              <OverviewCpuUtilization
                chartSeries={cpuUtilizations.map((element) => element.utilizationPercentage)}
                labels={cpuUtilizations.map((element) => element.label)}
                sx={{ height: '100%' }}
              />
            </Grid>
            <Grid
              xs={12}
              md={6}
              lg={4}
            >
              {/* ... (additional Grid components) */}
            </Grid>
          </Grid>
        </Container>
      </Box>
    </>
  );
};

Page.getLayout = (page) => (
  <DashboardLayout>
    {page}
  </DashboardLayout>
);

export default Page;
