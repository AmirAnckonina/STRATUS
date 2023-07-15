import { useCallback, useMemo, useState } from 'react';
import Head from 'next/head';
import { subDays, subHours } from 'date-fns';
import ArrowDownOnSquareIcon from '@heroicons/react/24/solid/ArrowDownOnSquareIcon';
import ArrowUpOnSquareIcon from '@heroicons/react/24/solid/ArrowUpOnSquareIcon';
import PlusIcon from '@heroicons/react/24/solid/PlusIcon';
import { Box, Button, Container, Stack, SvgIcon, Typography } from '@mui/material';
import { useSelection } from 'src/hooks/use-selection';
import { Layout as DashboardLayout } from 'src/layouts/dashboard/layout';
import { AlertsTable } from 'src/sections/alert/alert-table';
import { AlertsSearch } from 'src/sections/alert/alert-search';
import { applyPagination } from 'src/utils/apply-pagination';

const now = new Date();

const useAlerts = (page, rowsPerPage) => {
  return useMemo(
    () => {
      return applyPagination([], page, rowsPerPage);
    },
    [page, rowsPerPage]
  );
};

const useAlertIds = (Alerts) => {
  return useMemo(
    () => {
      return Alerts.map((Alert) => Alert.id);
    },
    [Alerts]
  );
};

const Page = () => {
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(5);
  const Alerts = useAlerts(page, rowsPerPage);
  const AlertsIds = useAlertIds(Alerts);
  const AlertsSelection = useSelection(AlertsIds);

  const handlePageChange = useCallback(
    (event, value) => {
      setPage(value);
    },
    []
  );

  const handleRowsPerPageChange = useCallback(
    (event) => {
      console.log('rows value:', event);
      setRowsPerPage(event);
    },
    []
  );

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
                      <PlusIcon />
                    </SvgIcon>
                  )}
                  variant="contained"
                >
                  Add
                </Button>
              </div>
            </Stack>
            <AlertsSearch />
            <AlertsTable
              count={0}
              items={Alerts}
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
    </>
  );
};

Page.getLayout = (page) => (
  <DashboardLayout>
    {page}
  </DashboardLayout>
);

export default Page;
