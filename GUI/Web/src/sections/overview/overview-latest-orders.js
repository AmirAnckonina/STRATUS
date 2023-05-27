import { format } from 'date-fns';
import PropTypes from 'prop-types';
import ArrowRightIcon from '@heroicons/react/24/solid/ArrowRightIcon';
import axios from 'axios';
import React, { useEffect, useState } from 'react'; 

import {
  Box,
  Button,
  Card,
  CardActions,
  CardHeader,
  Divider,
  SvgIcon,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow
} from '@mui/material';
import { Scrollbar } from 'src/components/scrollbar';
import { SeverityPill } from 'src/components/severity-pill';

const statusMap = {
  pending: 'warning',
  delivered: 'success',
  refunded: 'error'
};

export const OverviewLatestOrders = (props) => {
  const { orders = [], sx } = props;
  const [machines, setData] = useState([])
  useEffect(() => {
  axios.get('https://localhost:7094/GetUserInstanceData')
  .then(response => {
    setData(response.data.data)
    console.log("Response test", response.data.data);
  })
  .catch(error => console.error(error));
  },[]);


  return (   
    <Card sx={sx}>
      <CardHeader title="Latest Machines" />
      <Scrollbar sx={{ flexGrow: 1 }}>
        <Box sx={{ minWidth: 800 }}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell sortDirection="desc">
                  Id
                </TableCell>
                <TableCell>
                  OS
                </TableCell>
                <TableCell>
                  Price
                </TableCell>
                <TableCell>
                  CPU
                </TableCell>
                <TableCell>
                Total Storage Size
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {machines.map((data) => {

                return (
                  <TableRow
                    hover
                    key={data.id}
                  >
                     <TableCell>
                      {data.id}
                    </TableCell>
                    <TableCell>
                      {data.operatingSystem}
                    </TableCell>
                    <TableCell>
                      {data.price}
                    </TableCell>
                    <TableCell>
                      {data.cpuSpecifications}
                    </TableCell>
                    <TableCell>
                      {data.totalStorageSize} GB
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        </Box>
      </Scrollbar>
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
          variant="text"
        >
          View all
        </Button>
      </CardActions>
    </Card>
  );
};

OverviewLatestOrders.prototype = {
  orders: PropTypes.array,
  sx: PropTypes.object
};
