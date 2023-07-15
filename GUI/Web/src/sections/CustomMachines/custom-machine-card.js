import PropTypes from 'prop-types';
import ArrowDownOnSquareIcon from '@heroicons/react/24/solid/ArrowDownOnSquareIcon';
import DollarIcon from '@heroicons/react/24/solid/CurrencyDollarIcon';
import { Button } from '@mui/material';
import ShoppingBagIcon from '@heroicons/react/24/solid/ShoppingBagIcon';
import { Avatar, Box, Card, CardContent, Divider, Stack, SvgIcon, Typography } from '@mui/material';


export const CustomMachineCard = (props) => {
  const { machine } = props;

  const handleBuyNowClick = () => {
    window.location.href = 'https://aws.amazon.com/ec2/pricing/on-demand/';
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
          
          Type: {machine.type}
        </Typography>
        <Typography
  align="center"
  variant="body1"
>
  Operating System: {machine.operatingSystem}
  <br />
  Cpu Specifications: {machine.cpuSpecifications}
  <br />
  Storage: {machine.storage}
</Typography>
      </CardContent>
      <Box sx={{ flexGrow: 1 }} />
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
          <SvgIcon
            color="action"
            fontSize="small"
          >
            <DollarIcon />
          </SvgIcon>
          <Typography
            color="text.secondary"
            display="inline"
            variant="body2"
          >
            Price: {machine.price}$ per {machine.unit}
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
