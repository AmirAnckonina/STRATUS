import PropTypes from 'prop-types';
import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Alert } from '@mui/material';
import { Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@mui/material';

import {
  Box,
  Button,
  Card,
  CardActions,
  CardHeader,
  Checkbox,
  Divider,
  SvgIcon,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
} from '@mui/material';
import { Scrollbar } from 'src/components/scrollbar';
import ArrowRightIcon from '@mui/icons-material/ArrowRight';

export const OverviewLatestMachines = ({ onMachineSelect }) => {
  const [machines, setData] = useState([]);
  const [selectedMachine, setSelectedMachine] = useState(null);
  const [isDialogOpen, setIsDialogOpen] = useState(false);


  useEffect(() => {
    axios.get('https://localhost:7094/GetUserInstanceData')
      .then(response => {
        const machinesWithSelection = response.data.data.map(machine => ({
          ...machine,
          isSelected: false,
        }));
        setData(machinesWithSelection);
        console.log("Response test", machinesWithSelection);
      })
      .catch(error => console.error(error));
  }, []);

  const handleMachineChange = (event, machine) => {
    if (event.target.checked) {
      if(selectedMachine === null || machine.instanceAddress === selectedMachine) { 
          machine.isSelected = true;
          onMachineSelect(machine.instanceAddress);
          setSelectedMachine(machine.instanceAddress);
      }
      else {
        setIsDialogOpen(true);
      }
 
    } else {
      if(machine.instanceAddress === selectedMachine) {
        setSelectedMachine(null);
        machine.isSelected = false;
        onMachineSelect(null);
      }

    }
  };

  return (
    <Card>
      <CardHeader title="Latest Machines" />
      <Scrollbar sx={{ flexGrow: 1 }}>
        <Box sx={{ minWidth: 800 }}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>
                  Select
                </TableCell>
                <TableCell sortDirection="desc">
                  Id
                </TableCell>
                <TableCell>
                  IP
                </TableCell>
                <TableCell>
                  Type
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
                 Storage
                </TableCell>
                <TableCell>
                 Memory
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {machines.map((data) => (
                <TableRow
                  
                  key={data.instanceAddress}
                  sx={{
                    backgroundColor: data.isSelected ? 'lightblue' : 'transparent',
                  }}
                >
                  <TableCell>
                  <Checkbox
                    checked={data.isSelected} // Use the isSelected field to determine if checkbox is checked
                    onChange={(event) => handleMachineChange(event, data)}
/>

                  </TableCell>
                  <TableCell>
                    {data.instanceId}
                  </TableCell>
                  <TableCell>
                    {data.instanceAddress}
                  </TableCell>
                  <TableCell>
                    {data.type}
                  </TableCell>
                  <TableCell>
                    {data.specifications.operatingSystem.toString()}
                  </TableCell>
                  <TableCell>
                    {data.specifications.price.priceAsString.toString()}
                  </TableCell>
                  <TableCell>
                    {data.specifications.vcpu.toString()}
                  </TableCell>
                  <TableCell>
                    {data.specifications.storage.asString.toString()} 
                  </TableCell>
                  <TableCell>
                    {data.specifications.memory.asString.toString()} 
                  </TableCell>
                </TableRow>
              ))}
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
      <Dialog open={isDialogOpen} onClose={() => setIsDialogOpen(false)}>
  <DialogTitle>
    <Box display="flex" alignItems="center">
      <Alert severity="error" sx={{ marginRight: '8px' }} />
      Validation Error
    </Box>
  </DialogTitle>
  <DialogContent>
    <DialogContentText>
      At most one instance can be selected!
    </DialogContentText>
  </DialogContent>
  <DialogActions>
    <Button onClick={() => setIsDialogOpen(false)}>OK</Button>
  </DialogActions>
</Dialog>


    </Card>
  );
};

OverviewLatestMachines.propTypes = {
  onMachineSelect: PropTypes.func.isRequired,
};
