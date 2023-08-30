import PropTypes from 'prop-types';
import { ArrowUpIcon, ArrowDownIcon } from '@heroicons/react/24/solid';
import MinusIcon from '@heroicons/react/24/solid/MinusSmallIcon'
import { Button, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography, useTheme } from '@mui/material';
import ShoppingBagIcon from '@heroicons/react/24/solid/ShoppingBagIcon';
import { Avatar, Box, Card, CardContent, Divider, Stack, SvgIcon } from '@mui/material';

export const CustomMachineCard = (props) => {
  const { machine } = props;
  const theme = useTheme();

  const handleBuyNowClick = () => {
    window.location.href = 'https://aws.amazon.com/ec2/pricing/on-demand/';
  };

  const tableData = [
    { fieldName: 'Type', prevValue: machine.instanceDetails.type, alternativeValue: machine.alternativeInstance.instanceType, difference: 5 },
    { fieldName: 'Operating System', prevValue: machine.instanceDetails.specifications.operatingSystem.toString(), alternativeValue: machine.alternativeInstance.specifications.operatingSystem.toString(), difference: -10 },
    { fieldName: 'Memory', prevValue: machine.instanceDetails.specifications.memory.asString.toString(), alternativeValue: machine.alternativeInstance.specifications.memory.asString.toString(), difference: machine.memoryDiff },
    { fieldName: 'Storage', prevValue: machine.instanceDetails.specifications.storage.asString.toString(), alternativeValue:  machine.alternativeInstance.specifications.storage.asString.toString(), difference: -20 },
    { fieldName: 'vCpu', prevValue: machine.instanceDetails.specifications.vcpu, alternativeValue: machine.alternativeInstance.specifications.vcpu, difference: machine.vCpuDiff },
    { fieldName: 'Price', prevValue: machine.instanceDetails.specifications.price.priceAsString, alternativeValue: machine.alternativeInstance.specifications.price.priceAsString, difference: machine.priceDiff },
  ];

  // Function to determine arrow color based on value
  const getArrowColor = (value, index) => {
    return index === 3 || index === 0 || index === 1  || value === 0 ? theme.palette.grey.main : value < 0 ? theme.palette.error.main : theme.palette.success.main;
  };

  return (
    <Card
      sx={{
        display: 'flex',
        flexDirection: 'column',
        height: '100%'
      }}
    >
      <CardContent>
        <Box
          sx={{
            display: 'flex',
            justifyContent: 'center',
            pb: 3
          }}
        >
          <Avatar
            src={'/assets/logos/aws-logo.jpeg'}
            variant="square"
            sx={{ width: 125, height: 100 }}
          />
        </Box>
        <Typography
          align="center"
          gutterBottom
          variant="h5"
        >
          Region: {machine.alternativeInstance.region}
        </Typography>        
      </CardContent>
      <Box sx={{ flexGrow: 1 }} />
      <Divider />
      <TableContainer component={Paper} sx={{ mt: 2 }}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Field Name</TableCell>
              <TableCell>Current</TableCell>
              <TableCell>Alternative</TableCell>
              <TableCell>Difference</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {tableData.map((data, index) => (
              <TableRow key={data.fieldName}>
                <TableCell>{data.fieldName}</TableCell>
                <TableCell>{data.prevValue}</TableCell>
                <TableCell>{data.alternativeValue}</TableCell>
                <TableCell>
                  <Box sx={{ display: 'flex', alignItems: 'center', color: index === 5? getArrowColor(data.difference * -1, index) :  getArrowColor(data.difference, index)}}>
                    {index === 3 || index === 0 || index === 1 ||  data.difference === 0 ? <MinusIcon/> : data.difference > 0 ? <ArrowUpIcon fontSize="small" /> : <ArrowDownIcon fontSize="small"/>}
                    
                      {index === 3 || index === 0 || index === 1 ? null : `${Math.abs(data.difference.toFixed(2))}%`}                    
                  </Box>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <Divider />
      <Stack
        alignItems="center"
        direction="row"
        justifyContent="space-between"
        spacing={2}
        sx={{ p: 2 }}
      >
         <Stack
          alignItems="center"
          direction="row"
          spacing={1}
        >
          <Typography
            color="text.secondary"
            display="inline"
            variant="body2"
          >
            {/* You can save up to 50% in month ! */}
          </Typography>
        </Stack>
        <Stack
          alignItems="center"
          direction="row"
          spacing={1}
        >
          <SvgIcon
            color="action"
            fontSize="small"
          >
            <ShoppingBagIcon />
          </SvgIcon>
          <Button variant="outlined" size="small" onClick={handleBuyNowClick}>
           Buy Now
          </Button>
        </Stack>
      </Stack>
    </Card>
  );
};

CustomMachineCard.propTypes = {
  machine: PropTypes.object.isRequired
};
