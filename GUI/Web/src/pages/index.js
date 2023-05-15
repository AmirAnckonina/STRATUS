import Head from 'next/head';
import { subDays, subHours } from 'date-fns';
import { Box, Container, Unstable_Grid2 as Grid } from '@mui/material';
import { Layout as DashboardLayout } from 'src/layouts/dashboard/layout';
import { OverviewBudget } from 'src/sections/overview/overview-budget';
import { OverviewLatestOrders } from 'src/sections/overview/overview-latest-orders';
import { OverviewLatestProducts } from 'src/sections/overview/overview-latest-products';
import { OverviewSales } from 'src/sections/overview/overview-sales';
import { OverviewTasksProgress } from 'src/sections/overview/overview-tasks-progress';
import { OverviewTotalCustomers } from 'src/sections/overview/overview-total-customers';
import { OverviewTotalProfit } from 'src/sections/overview/overview-total-profit';
import { OverviewTraffic } from 'src/sections/overview/overview-traffic';
import React, { useState, useEffect  } from 'react';
import axios from 'axios';
import {
  Typography,
  Select,
  MenuItem
} from '@mui/material';


const now = new Date();

const Page = () => {
  const [statistics, setStatistics] = useState([])
  const [machines, setMachine] = useState([])
  const [selectedMachine, setSelectedMachine] = useState('');

  useEffect(() => {
  axios.get('https://localhost:7094/GetUserInstanceData')
  .then(response => {
    const data = response.data.data;
      setMachine(data);
      if (data.length > 0) {
        setSelectedMachine(data[0].id);
      }

  })
  .catch(error => console.error(error));
  },[]);
   


  const handleMachineChange = (event) => {
    setSelectedMachine(event.target.value);


  axios.get('https://localhost:7094/GetInstanceCPUStatistics')
  .then(response => {
    const statistics = response.data.data.filter(machine => machine.id === selectedMachine);
    console.log("current1",  statistics)
    console.log("current2",  response.data.data)
    setStatistics(response.data.data[0]); //todo : need to use filter machine insteaf of data[0]

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
          <Typography variant="h4" mb={3}>
            Choose Machine:
            <Select value={selectedMachine} onChange={handleMachineChange} sx={{ ml: 2 }}>
              {machines.map((machine) => (
              <MenuItem key={machine} value={machine.id}>
                 {machine.id}
             </MenuItem>
             ))}
            </Select>
          </Typography>
          <Typography variant="h6" mb={3}>
            Current Machine: {selectedMachine}
          </Typography>
        <Grid
          container
          spacing={3}
        >
          <Grid
            xs={12}
            sm={6}
            lg={3}
          >
            <OverviewBudget
              difference={12}
              positive
              sx={{ height: '100%' }}
              value={statistics.minimum ? statistics.minimum.toFixed(2) : "N/A"}
            />
          </Grid>
          <Grid
            xs={12}
            sm={6}
            lg={3}
          >
            <OverviewTotalCustomers
              difference={16}
              positive={false}
              sx={{ height: '100%' }}
              value={statistics.maximum ? statistics.maximum.toFixed(2) : "N/A"}
            />
          </Grid>
          <Grid
            xs={12}
            sm={6}
            lg={3}
          >
            <OverviewTasksProgress
              sx={{ height: '100%' }}
              value={statistics.average ? statistics.average.toFixed(2) : "N/A"}
            />
          </Grid>
          <Grid
            xs={12}
            sm={6}
            lg={3}
          >
            <OverviewTotalProfit
              sx={{ height: '100%' }}
              value={statistics.sum ? statistics.sum.toFixed(2) : "N/A"}
            />
          </Grid>
          <Grid
            xs={12}
            lg={8}
          >
            <OverviewSales
              chartSeries={[
                {
                  name: 'This year',
                  data: [18, 16, 5, 8, 3, 14, 14, 16, 17, 19, 18, 20]
                },
                {
                  name: 'Last year',
                  data: [12, 11, 4, 6, 2, 9, 9, 10, 11, 12, 13, 13]
                }
              ]}
              sx={{ height: '100%' }}
            />
          </Grid>
          <Grid
            xs={12}
            md={6}
            lg={4}
          >
            <OverviewTraffic
              chartSeries={[63, 15, 22]}
              labels={['Desktop', 'Tablet', 'Phone']}
              sx={{ height: '100%' }}
            />
          </Grid>
          <Grid
            xs={12}
            md={6}
            lg={4}
          >
            {/* <OverviewLatestProducts
              products={[
                {
                  id: '5ece2c077e39da27658aa8a9',
                  image: '/assets/products/product-1.png',
                  name: 'Healthcare Erbology',
                  updatedAt: subHours(now, 6).getTime()
                },
                {
                  id: '5ece2c0d16f70bff2cf86cd8',
                  image: '/assets/products/product-2.png',
                  name: 'Makeup Lancome Rouge',
                  updatedAt: subDays(subHours(now, 8), 2).getTime()
                },
                {
                  id: 'b393ce1b09c1254c3a92c827',
                  image: '/assets/products/product-5.png',
                  name: 'Skincare Soja CO',
                  updatedAt: subDays(subHours(now, 1), 1).getTime()
                },
                {
                  id: 'a6ede15670da63f49f752c89',
                  image: '/assets/products/product-6.png',
                  name: 'Makeup Lipstick',
                  updatedAt: subDays(subHours(now, 3), 3).getTime()
                },
                {
                  id: 'bcad5524fe3a2f8f8620ceda',
                  image: '/assets/products/product-7.png',
                  name: 'Healthcare Ritual',
                  updatedAt: subDays(subHours(now, 5), 6).getTime()
                }
              ]}
              sx={{ height: '100%' }}
            /> */}
          </Grid>
          <Grid
            xs={12}
            md={12}
            lg={16}
          >
            <OverviewLatestOrders
              orders = {[]}
            />
          </Grid>
        </Grid>
      </Container>
    </Box>
  </>
)};

Page.getLayout = (page) => (
  <DashboardLayout>
    {page}
  </DashboardLayout>
);

export default Page;
